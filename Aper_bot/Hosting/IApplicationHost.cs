namespace Aper_bot.Hosting
{
    public interface IApplicationHost
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="args"></param>
        void Init();

        void Run();
    }
}