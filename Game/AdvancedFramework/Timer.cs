using System.Diagnostics;

namespace AdvancedFramework
{
    public class Timer
	{
    	private Stopwatch stopwatch;
        private float elapsedTime;
        private bool paused;
        
        public bool Paused
        {
        	get { return paused; }
        	set
        	{
        		paused = value;
        		
        		if (value)
        			stopwatch.Stop();
        		else
        			stopwatch.Start();
        	}
        }

        public Timer()
		{
        	stopwatch = new Stopwatch();
        	stopwatch.Stop();
        }
        
        public void Reset()
        {
        	stopwatch.Reset();
        	stopwatch.Start();
        	
        	paused = false;
        }

        public float Elapsed()
        {
            elapsedTime = stopwatch.ElapsedMilliseconds / 1000f;
            this.Reset();
            
            return elapsedTime;
        }
        
        public float ElapsedNoReset()
        {
        	return stopwatch.ElapsedMilliseconds / 1000f;
        }
    }
}
