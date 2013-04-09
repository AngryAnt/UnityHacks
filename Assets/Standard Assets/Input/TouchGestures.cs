using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


// TODO: Revert workaround when namespaces + default params work again
using UnityAssets;
//namespace UnityAssets
//{
	public enum GestureType
	{
		Swipe = 1,
		DoubleSwipe = 2,
		TrippleSwipe = 4,
		Pinch = 8,
		Tap = 16
	}


	public struct Gesture
	{
		public GestureType type;
		public Vector2 start, direction;


		public Gesture (GestureType type, Vector2 start, Vector2 direction)
		{
			this.type = type;
			this.start = start;
			this.direction = direction;
		}
	}


	public class TouchGestures : MonoBehaviour
	{
		class TrackedTouch
		{
			public int fingerId;
			public float startTime;
			public List<Vector2> positions = new List<Vector2> ();


			public TrackedTouch (int id, Vector2 position)
			{
				fingerId = id;
				positions.Add (position);
				startTime = Time.time;
			}


			public Vector2 Start
			{
				get
				{
					return positions[0];
				}
			}


			public Vector2 End
			{
				get
				{
					return positions[positions.Count - 1];
				}
			}


			public Vector2 Travel
			{
				get
				{
					if (positions.Count < 2)
					{
						return Vector2.zero;
					}

					return End - Start;
				}
			}


			public Vector2 NormalizedTravel
			{
				get
				{
					Vector2 travel = Travel;
					return travel == Vector2.zero ? Vector2.zero : new Vector2 (travel.x / Screen.width, travel.y / Screen.height);
				}
			}
		}


		public float maxDuration = 0.25f, minSwipeScreenTravel = 0.2f, minPinchScreenTravel = 0.1f, maxTapScreenTravel = 0.05f, maxTapDuration = 0.5f;
		public MonoBehaviour[] receivers;
		public GestureType trackingMask = 0;
		public bool trackMultiple = true;


		Dictionary<int, TrackedTouch> trackedTouches = new Dictionary<int, TrackedTouch> ();
		float maxParallelDotDeviation = 0.2f, maxOpposedDotDeviant = 0.5f;


		bool SendGesture (GestureType type, Vector2 start, Vector2 direction)
		{
			foreach (MonoBehaviour receiver in receivers)
			{
				receiver.SendMessage ("OnGesture", new Gesture (type, start, direction));
			}

			if (!trackMultiple)
			{
				trackedTouches.Clear ();
				return false;
			}

			return true;
		}


		void Update ()
		{
			Record ();

			AnalysePinch ();
			AnalyseTrippleSwipe ();
			AnalyseDoubleSwipe ();
			AnalyseSwipe ();
		}


		TrackedTouch[] GetValidSwipes (float minNormalizedTravel, float maxNormalizedTravel = 0.0f)
		{
			return (
				from touch in trackedTouches.Values
					where touch.NormalizedTravel.magnitude > minNormalizedTravel && (maxNormalizedTravel == 0.0f || touch.NormalizedTravel.magnitude < maxNormalizedTravel)
					orderby touch.Travel.sqrMagnitude descending
				select touch
			).ToArray ();
		}


		TrackedTouch[] GetValidPartnerSwipes (TrackedTouch baseTouch, ICollection<TrackedTouch> selection, float targetDot, float maxDeviation)
		{
			return (
				from touch in selection
					where touch != baseTouch &&
						Mathf.Abs (targetDot - Vector2.Dot (baseTouch.Travel.normalized, touch.Travel.normalized)) < maxDeviation
					orderby Mathf.Abs (targetDot - Vector2.Dot (baseTouch.Travel.normalized, touch.Travel.normalized)) ascending
				select touch
			).ToArray ();
		}


		void AnalysePinch ()
		{
			if ((trackingMask & GestureType.Pinch) == 0)
			{
				return;
			}

			TrackedTouch[] swipes = GetValidSwipes (minPinchScreenTravel);

			if (swipes.Length > 1)
			{
				for (int i = 0; i < swipes.Length; i++)
				{
					TrackedTouch[] partnerSwipes = GetValidPartnerSwipes (swipes[i], swipes, -1.0f, maxOpposedDotDeviant);

					if (partnerSwipes.Length > 0)
					{
						trackedTouches.Remove (swipes[i].fingerId);
						trackedTouches.Remove (partnerSwipes[0].fingerId);

						Vector2 travelA = swipes[i].Travel, travelB = partnerSwipes[0].Travel;

						if (SendGesture (
							GestureType.Pinch,
							swipes[i].Start + (partnerSwipes[0].Start - swipes[i].Start) * 0.5f,
							Vector2.one * (travelA.magnitude + travelB.magnitude) *
								((swipes[i].Start - partnerSwipes[0].Start).sqrMagnitude < (swipes[i].End - partnerSwipes[0].End).sqrMagnitude ? 1.0f : -1.0f)
						))
						{
							AnalysePinch ();
						}
						else
						{
							return;
						}
					}
				}
			}
		}


		void AnalyseTrippleSwipe ()
		{
			if ((trackingMask & GestureType.TrippleSwipe) == 0)
			{
				return;
			}

			TrackedTouch[] swipes = GetValidSwipes (minSwipeScreenTravel);

			if (swipes.Length > 2)
			{
				for (int i = 0; i < swipes.Length; i++)
				{
					TrackedTouch[] partnerSwipes = GetValidPartnerSwipes (swipes[i], swipes, 1.0f, maxParallelDotDeviation);

					if (partnerSwipes.Length > 1)
					{
						trackedTouches.Remove (swipes[i].fingerId);
						trackedTouches.Remove (partnerSwipes[0].fingerId);
						trackedTouches.Remove (partnerSwipes[1].fingerId);

						Vector2 travelA = swipes[i].Travel, travelB = partnerSwipes[0].Travel, travelC = partnerSwipes[1].Travel;

						if (SendGesture (
							GestureType.TrippleSwipe,
							swipes[i].Start,
							(travelA + travelB + travelC).normalized * (travelA.magnitude + travelB.magnitude + travelC.magnitude) / 3.0f
						))
						{
							AnalyseTrippleSwipe ();
						}
						else
						{
							return;
						}
					}
				}
			}
		}


		void AnalyseDoubleSwipe ()
		{
			if ((trackingMask & GestureType.DoubleSwipe) == 0)
			{
				return;
			}

			TrackedTouch[] swipes = GetValidSwipes (minSwipeScreenTravel);

			if (swipes.Length > 1)
			{
				for (int i = 0; i < swipes.Length; i++)
				{
					TrackedTouch[] partnerSwipes = GetValidPartnerSwipes (swipes[i], swipes, 1.0f, maxParallelDotDeviation);

					if (partnerSwipes.Length > 0)
					{
						trackedTouches.Remove (swipes[i].fingerId);
						trackedTouches.Remove (partnerSwipes[0].fingerId);

						Vector2 travelA = swipes[i].Travel, travelB = partnerSwipes[0].Travel;

						if (SendGesture (GestureType.DoubleSwipe, swipes[i].Start, (travelA + travelB).normalized * (travelA.magnitude + travelB.magnitude) * 0.5f))
						{
							AnalyseDoubleSwipe ();
						}
						else
						{
							return;
						}
					}
				}
			}
		}


		void AnalyseSwipe ()
		{
			if ((trackingMask & GestureType.Swipe) == 0)
			{
				return;
			}

			TrackedTouch[] swipes = GetValidSwipes (minSwipeScreenTravel);

			if (swipes.Length > 0)
			{
				trackedTouches.Remove (swipes[0].fingerId);
				SendGesture (GestureType.Swipe, swipes[0].Start, swipes[0].Travel);
			}
		}


		void Record ()
		{
			foreach (Touch touch in Input.touches)
			{
				if (trackedTouches.ContainsKey (touch.fingerId))
				{
					if (Time.time - trackedTouches[touch.fingerId].startTime > maxDuration)
					{
						trackedTouches.Remove (touch.fingerId);
					}
					else
					{
						switch (touch.phase)
						{
							case TouchPhase.Ended:
								TrackedTouch trackedTouch = trackedTouches[touch.fingerId];

								if (
									Time.time - trackedTouch.startTime < maxTapDuration &&
									trackedTouch.NormalizedTravel.magnitude < maxTapScreenTravel
								)
								{
									SendGesture (GestureType.Tap, trackedTouch.Start, trackedTouch.Travel);
								}

								trackedTouches.Remove (touch.fingerId);
							break;
							case TouchPhase.Canceled:
								trackedTouches.Remove (touch.fingerId);
							break;
							case TouchPhase.Moved:
								trackedTouches[touch.fingerId].positions.Add (touch.position);
							break;
						}
					}
				}
				else if (touch.phase == TouchPhase.Began)
				{
					trackedTouches[touch.fingerId] = new TrackedTouch (touch.fingerId, touch.position);
				}
			}
		}
	}
//}
