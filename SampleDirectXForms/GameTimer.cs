namespace SampleDirectXForms;

using System;
using System.Diagnostics;

internal class GameTimer
{
    private readonly double secondsPerTick = 1.0 / Stopwatch.Frequency;
    private long baseTime;
    private long currentTime;
    private double mDeltaTime;
    private long pausedTime;
    private long previousTime;
    private bool stopped;
    private long stopTime;
    private readonly Stopwatch stopwatch = Stopwatch.StartNew();

    public float DeltaTime => (float)this.mDeltaTime;

    public float GameTime
    {
        get
        {
            var currTime = this.stopped ? this.stopTime : this.currentTime;
            return (float)((currTime - this.pausedTime - this.baseTime) * this.secondsPerTick);
        }
    }

    public void Reset()
    {
        var currTime = this.stopwatch.ElapsedTicks;

        this.baseTime = currTime;
        this.previousTime = currTime;
        this.stopTime = 0;
        this.stopped = false;
    }

    public void Start()
    {
        if (!this.stopped)
        {
            return;
        }

        var startTime = this.stopwatch.ElapsedTicks;

        this.pausedTime += startTime - this.stopTime;
        this.previousTime = startTime;
        this.stopTime = 0;
        this.stopped = false;
    }

    public void Stop()
    {
        if (this.stopped)
        {
            return;
        }

        var currTime = this.stopwatch.ElapsedTicks;

        this.stopTime = currTime;
        this.stopped = true;
    }

    public void Tick()
    {
        if (this.stopped)
        {
            this.mDeltaTime = 0.0;
            return;
        }

        var currTime = this.stopwatch.ElapsedTicks;

        this.currentTime = currTime;
        this.mDeltaTime = Math.Max((this.currentTime - this.previousTime) * this.secondsPerTick, 0.0);
        this.previousTime = this.currentTime;
    }
}