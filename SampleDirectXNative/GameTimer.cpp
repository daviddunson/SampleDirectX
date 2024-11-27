#include "Common.h"
#include "GameTimer.h"

GameTimer::GameTimer() :
    mSecondsPerCount(0),
    mDeltaTime(0),
    mBaseTime(0),
    mPausedTime(0),
    mStopTime(0),
    mPrevTime(0),
    mCurrTime(0),
    mStopped(false)
{
    __int64 countsPerSec;
    QueryPerformanceFrequency(reinterpret_cast<LARGE_INTEGER*>(&countsPerSec));
    mSecondsPerCount = 1.0 / static_cast<double>(countsPerSec);
}

float GameTimer::GameTime() const
{
    const __int64 currTime = (mStopped ? mStopTime : mCurrTime);
    return static_cast<float>((currTime - mPausedTime - mBaseTime) * mSecondsPerCount);
}

float GameTimer::DeltaTime() const
{
    return static_cast<float>(mDeltaTime);
}

void GameTimer::Reset()
{
    __int64 currTime;
    QueryPerformanceCounter(reinterpret_cast<LARGE_INTEGER*>(&currTime));

    mBaseTime = currTime;
    mPrevTime = currTime;
    mStopTime = 0;
    mStopped = false;
}

void GameTimer::Start()
{
    if (!mStopped)
    {
        return;
    }

    __int64 startTime;
    QueryPerformanceCounter(reinterpret_cast<LARGE_INTEGER*>(&startTime));

    mPausedTime += (startTime - mStopTime);
    mPrevTime = startTime;
    mStopTime = 0;
    mStopped = false;
}

void GameTimer::Stop()
{
    if (mStopped)
    {
        return;
    }

    __int64 currTime;
    QueryPerformanceCounter(reinterpret_cast<LARGE_INTEGER*>(&currTime));

    mStopTime = currTime;
    mStopped = true;
}

void GameTimer::Tick()
{
    if (mStopped)
    {
        mDeltaTime = 0.0;
        return;
    }

    __int64 currTime;
    QueryPerformanceCounter(reinterpret_cast<LARGE_INTEGER*>(&currTime));
    mCurrTime = currTime;

    mDeltaTime = std::max((mCurrTime - mPrevTime) * mSecondsPerCount, 0.0);
    mPrevTime = mCurrTime;
}
