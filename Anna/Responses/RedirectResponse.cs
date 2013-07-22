using Anna.Request;

namespace Anna.Responses
{
    public class RedirectResponse : Response
    {
        private readonly string url;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectResponse"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="url">The URL to redirect to.</param>        
        public RedirectResponse(RequestContext context, string url)
            : base(context, 302)
        {
            this.url = url;
        }

        public override void Send()
        {
            ListenerResponse.Redirect(url);
            ListenerResponse.Close();
        }
    }
}