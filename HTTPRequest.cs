class HTTPRequest : CanvasLayer
{
    public override void _Ready()
    {
        GetNode("HTTPRequest").Connect("request_completed", this, "OnRequestCompleted");
        GetNode("Button").Connect("pressed", this, "OnButtonPressed ");
    }

    public void OnButtonPressed()
    {
        HTTPRequest httpRequest = GetNode<HTTPRequest>("HTTPRequest");
        httpRequest.Request("https://rootingwebapi.azurewebsites.net/WeatherForecast", new string[] { "user-agent: MarmaladeUserAgent" });
    }

    public void OnRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));
        GD.Print(json.Result);
    }

    public void MakePostRequest(string url, object data_to_send, bool use_ssl)
    {
        string query = JSON.Print(data_to_send);
        HTTPRequest httpRequest = GetNode<HTTPRequest>("HTTPRequest");
        string[] headers = new string[] { "Content-Type: application/json" };
        httpRequest.Request(url, headers, use_ssl, HTTPClient.Method.Post, query);
    }
}
