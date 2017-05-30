using System;
using System.Net;
using System.IO;


class requestObject
{
    HttpWebRequest request;
    byte[] buffer;

    public requestObject(HttpWebRequest _r, byte[] _b)
    {
        request = _r;
        buffer = _b;
    }
    public byte[] GetBuffer() { return buffer; }
    public HttpWebRequest GetRequest() { return request; }
}

class HttpSendRecv
{
    public HttpSendRecv() { }

    public void Send(string _url, byte[] _data)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
        requestObject o = new requestObject(request, _data);

        request.ContentType = "application/x-www-form-urlencoded";
        request.Method = "POST";

        request.BeginGetRequestStream(new AsyncCallback(requestStreamCompleted), o);
    }

    public void Send(string _url)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
        byte[] _data = new byte[1];
        _data[0] = 0x00;
        requestObject o = new requestObject(request, _data);


        request.ContentType = "application/x-www-form-urlencoded";
        request.Method = "POST";


        request.BeginGetRequestStream(new AsyncCallback(requestStreamCompleted), o);
    }

    void requestStreamCompleted(IAsyncResult _result)
    {
        requestObject requestObject = (requestObject)_result.AsyncState;
        HttpWebRequest request = requestObject.GetRequest();
        byte[] buffer = requestObject.GetBuffer();

        Stream postStream = request.EndGetRequestStream(_result);
        postStream.Write(buffer, 0, buffer.Length);
        postStream.Close();
        //콜백함수 등록
        request.BeginGetResponse(new AsyncCallback(HttpResponseCompleted), requestObject);
    }

    //recv콜백 함수
    void HttpResponseCompleted(IAsyncResult _result)
    {
        requestObject request = (requestObject)_result.AsyncState;
        HttpWebResponse response = (HttpWebResponse)request.GetRequest().EndGetResponse(_result);

        Stream responseStream = response.GetResponseStream();
        StreamReader streamReader = new StreamReader(responseStream);
        string responseString = streamReader.ReadToEnd();
        streamReader.Close();
        responseStream.Close();
        response.Close();
        HTTPManager.getInstance().RECV_HTTP(responseString);
    }
}

