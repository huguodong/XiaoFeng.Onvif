﻿using System.Net;
using XiaoFeng.Xml;

namespace XiaoFeng.Onvif
{
    /// <summary>
    /// 云台服务
    /// </summary>
    public class PTZService
    {
        public static readonly string URL = "onvif/media_service";
        /// <summary>
        /// 获取云台状态
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="onvifUTCDateTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetStatus(string ip, string user, string pass, DateTime onvifUTCDateTime,string token)
        {
            string reqMessageStr = $@"
                                        <tptz:GetStatus xmlns:tptz=""http://www.onvif.org/ver20/ptz/wsdl"">
                                          <tptz:ProfileToken>{token}</tptz:ProfileToken>
                                        </tptz:GetStatus>";
            var result = await OnvifAuth.RemoteClient(ip, URL, reqMessageStr, user, pass, onvifUTCDateTime);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                var xnode_list = result.Html.XmlToEntity<XmlValue>();
            }
            else
            {
                /*请求失败*/
            }
            return default;
        }
        public static async Task<List<string>> AbsoluteMove(string ip, string user, string pass,
            DateTime onvifUTCDateTime, string token,double x, double y)
        {
            string reqMessageStr = $@"
                        <tptz:AbsoluteMove xmlns:tptz=""http://www.onvif.org/ver20/ptz/wsdl"">
                              <tptz:ProfileToken>{token}</tptz:ProfileToken>
                              <tptz:Position>
                                <tt:PanTilt x=""{x}"" y=""{y}"" space=""http://www.onvif.org/ver10/tptz/PanTiltSpaces/PositionGenericSpace"" />
                              </tptz:Position>
                        </tptz:AbsoluteMove>";
            var result = await OnvifAuth.RemoteClient(ip, URL, reqMessageStr, user, pass, onvifUTCDateTime);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                await Stop(ip, user, pass, onvifUTCDateTime, token);
                var xnode_list = result.Html.XmlToEntity<XmlValue>();
            }
            else
            {
                /*请求失败*/
            }
            return default;
        }
        /// <summary>
        /// 停止移动
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="onvifUTCDateTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<List<string>> Stop(string ip, string user, string pass,
           DateTime onvifUTCDateTime, string token)
        {
            string reqMessageStr = $@"
                        <tptz:Stop xmlns:tptz=""http://www.onvif.org/ver20/ptz/wsdl"">
                                <tptz:ProfileToken>{token}</tptz:ProfileToken>
                                <tptz:PanTilt>true</tptz:PanTilt>
                                <tptz:Zoom>true</tptz:Zoom>
                        </tptz:Stop>";
            var result = await OnvifAuth.RemoteClient(ip, URL, reqMessageStr, user, pass, onvifUTCDateTime);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                var xnode_list = result.Html.XmlToEntity<XmlValue>();
            }
            else
            {
                /*请求失败*/
            }
            return default;
        }
        /// <summary>
        /// 继续移动
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="onvifUTCDateTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<List<string>> ContinuousMove(string ip, string user, string pass,
          DateTime onvifUTCDateTime, string token, double x, double y, double z)
        {
            string reqMessageStr = $@"
                        <tptz:ContinuousMove xmlns:tptz=""http://www.onvif.org/ver20/ptz/wsdl"">
                          <tptz:ProfileToken>{token}</tptz:ProfileToken>
                          <tptz:Velocity>
                            <tt:PanTilt x=""{x}"" y=""{y}"" />
                            <tt:Zoom x=""{z}"" />
                          </tptz:Velocity>
                        </tptz:ContinuousMove>";
            var result = await OnvifAuth.RemoteClient(ip, URL, reqMessageStr, user, pass, onvifUTCDateTime);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                await Stop(ip, user, pass, onvifUTCDateTime, token);
                var xnode_list = result.Html.XmlToEntity<XmlValue>();
            }
            else
            {
                /*请求失败*/
            }
            return default;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="onvifUTCDateTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<List<string>> SetHomePosition(string ip, string user, string pass,
          DateTime onvifUTCDateTime, string token)
        {
            string reqMessageStr = $@"
                       <tptz:SetHomePosition xmlns:tptz=""http://www.onvif.org/ver20/ptz/wsdl"">
                          <tptz:ProfileToken>{token}</tptz:ProfileToken>
                        </tptz:SetHomePosition>";
            var result = await OnvifAuth.RemoteClient(ip, URL, reqMessageStr, user, pass, onvifUTCDateTime);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                await Stop(ip, user, pass, onvifUTCDateTime, token);
                var xnode_list = result.Html.XmlToEntity<XmlValue>();
            }
            else
            {
                /*请求失败*/
            }
            return default;
        }
        public static async Task<List<string>> GotoHomePosition(string ip, string user, string pass,
         DateTime onvifUTCDateTime, string token, double x, double y, double z)
        {
            string reqMessageStr = $@"
                       <tptz:GotoHomePosition xmlns:tptz=""http://www.onvif.org/ver20/ptz/wsdl"">
                          <tptz:ProfileToken>{token}</tptz:ProfileToken>
                          <tptz:Speed>
                            <tt:PanTilt x=""{x}"" y=""{y}"" space=""http://uri1"" />
                            <tt:Zoom x=""{z}"" space=""http://uri1"" />
                          </tptz:Speed>
                        </tptz:GotoHomePosition>";
            var result = await OnvifAuth.RemoteClient(ip, URL, reqMessageStr, user, pass, onvifUTCDateTime);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                await Stop(ip, user, pass, onvifUTCDateTime, token);
                var xnode_list = result.Html.XmlToEntity<XmlValue>();
            }
            else
            {
                /*请求失败*/
            }
            return default;
        }
        public static async Task<List<string>> RelativeMove(string ip, string user, string pass,
         DateTime onvifUTCDateTime, string token, double x, double y, double z)
        {
            string reqMessageStr = $@"
                       <tptz:RelativeMove xmlns:tptz=""http://www.onvif.org/ver20/ptz/wsdl"">
                          <tptz:ProfileToken>{token}</tptz:ProfileToken>
                          <tptz:Translation>
                            <tt:PanTilt x=""{x}"" y=""{y}"" />
                            <tt:Zoom x=""{z}"" />
                          </tptz:Translation>
                          <tptz:Speed>
                            <tt:PanTilt x=""{x}"" y=""{y}"" />
                            <tt:Zoom x=""{z}"" />
                          </tptz:Speed>
                        </tptz:RelativeMove>";
            var result = await OnvifAuth.RemoteClient(ip, URL, reqMessageStr, user, pass, onvifUTCDateTime);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                await Stop(ip, user, pass, onvifUTCDateTime, token);
                var xnode_list = result.Html.XmlToEntity<XmlValue>();
            }
            else
            {
                /*请求失败*/
            }
            return default;
        }
    }
}
