namespace Assets.Scripts
{
    using UnityEngine;

    public class Timer
    {
        public float TargetTime { get; set; }

        public bool TimerEnded { get; set; }

        public Timer(float targetTime)
        {
            TimerEnded = false;
            TargetTime = targetTime;
        }

        public void UpdateTimer()
        {
            if (!TimerEnded)
            {
                TargetTime -= Time.deltaTime;
            }

            if (TargetTime <= 0.0f)
            {
                TimerEnded = true;
            }
        }
    }
}