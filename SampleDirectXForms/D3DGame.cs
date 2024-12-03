namespace SampleDirectXForms;

using System.Threading;

internal class D3DGame(nint instanceHandle)
    : D3DWindow(instanceHandle)
{
    private readonly D3DRenderer renderer = new();
    private readonly GameTimer timer = new();
    private float frameCount;
    private bool isPaused;
    private float timeElapsed;

    public override void Init()
    {
        base.Init();
        this.renderer.Init();
        this.timer.Reset();
    }

    public void Tick()
    {
        this.timer.Tick();

        if (this.isPaused)
        {
            Thread.Sleep(100);
            return;
        }

        this.CalculateFrameStats();
        this.UpdateScene(this.timer.DeltaTime);
        this.DrawScene();

        Thread.Sleep(0);
    }

    protected virtual void DrawScene()
    {
    }

    protected override void OnActivated()
    {
        this.isPaused = false;
        this.timer.Start();
    }

    protected override void OnDeactivated()
    {
        this.isPaused = true;
        this.timer.Stop();
    }

    protected override void OnResize()
    {
        this.isPaused = false;
        this.timer.Start();
    }

    protected override void OnResizing()
    {
        this.isPaused = true;
        this.timer.Stop();
    }

    protected virtual void UpdateScene(float dt)
    {
    }

    private void CalculateFrameStats()
    {
        this.frameCount++;

        var gameTime = this.timer.GameTime;

        if (gameTime - this.timeElapsed < 1.0f)
        {
            return;
        }

        var fps = (float)this.frameCount;
        var frameTime = 1000.0f / fps;

        var text = $"{this.Caption} FPS: {fps:0} Frame Time: {frameTime:0.000000} Game Time: {gameTime:0}";
        NativeMethods.SetWindowText(this.Handle, text);

        this.frameCount = 0;
        this.timeElapsed += 1.0f;
    }
}