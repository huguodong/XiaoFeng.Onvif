﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using XiaoFeng.Xml;
using XiaoFeng;
using System.Text.RegularExpressions;

namespace XiaoFeng.Onvif
{
    /// <summary>
    /// 设备服务
    /// </summary>
    public class DeviceService
    {
        public static readonly string URL = "onvif/device_service";

        /// <summary>
        /// 设备发现-基于当前网卡的网段局域网扫描
        /// </summary>
        /// <param name="timeoutSecond">超时时长 单位为毫秒</param>
        /// <param name="netMask">网段</param>
        /// <returns></returns>
        public static async Task<List<DiscoveryOnvifInfo>> DiscoveryOnvif(int timeoutSecond, string netMask = "255.255.255.255")
        {
            var list = await Utility.UdpOnvifClient(timeoutSecond, netMask);
            #region BufferToObject数据处理
            var data = new List<DiscoveryOnvifInfo>();
            foreach (var item in list)
            {
                var info = item.Buffer.GetString();
                var ipAddrs = info.GetMatch(@"XAddrs>(?<a>[^<]+)");
                Uri uri = new Uri(ipAddrs);
                var port = uri.Port.ToString();
                var host = uri.Host;
                data.Add(new DiscoveryOnvifInfo
                {
                    Name = info.GetMatch(@"onvif://www.onvif.org/name/(?<a>[^\s]+)").UrlDecode(),
                    UUID = info.GetMatch(@"wsa:Address>(?<a>[^<]+)").Split(':').LastOrDefault(),
                    Hardware = info.GetMatch(@"onvif://www.onvif.org/hardware/(?<a>[^\s]+)").UrlDecode(),
                    Ipv4Address = ipv4,
                    Ipv6Address = ipv6,
                    ServiceAddress = ipAddrs,
                    Remote = item.RemoteEndPoint.Address.ToString(),
                    Port = item.RemoteEndPoint.Port,
                    Types = info.GetMatch(@"Types>(?<a>[^<]+)")
                });
            }
            #endregion
            return data;
        }

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        public static async Task<DateTime> GetSystemDateAndTime(IPEndPoint iPEndPoint)
        {
            string ip = iPEndPoint.Address.ToString();
            if (!PrototypeHelper.IsIP(ip)) throw new Exception($"IP:{ip}格式不正确");
            if (!Utility.CheckPingEnable(ip)) throw new Exception($"Onvif主机{ip}无响应");
            var onvifUTCDateTime = DateTime.Now;
            string reqMessageStr = "<tds:GetSystemDateAndTime/>";
            var result = await OnvifAuth.RemoteClient(iPEndPoint, URL, reqMessageStr, "user", "pass", onvifUTCDateTime);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                var xnode = result.Html.ReplacePattern(@"(<|/)[a-z\-]+:", "$1");
                var utc = XElement.Parse(xnode).Descendants("UTCDateTime")
                            .Select(x => new
                            {
                                //year=x.XPathSelectElement("//Date//Year").Value.ToCast<int>(),
                                year = x.Element("Date").Element("Year").Value.ToCast<int>(),
                                month = x.Element("Date").Element("Month").Value.ToCast<int>(),
                                day = x.Element("Date").Element("Day").Value.ToCast<int>(),
                                hour = x.Element("Time").Element("Hour").Value.ToCast<int>(),
                                minute = x.Element("Time").Element("Minute").Value.ToCast<int>(),
                                second = x.Element("Time").Element("Second").Value.ToCast<int>()
                            }).FirstOrDefault();
                if (utc != null)
                    return new DateTime(utc.year, utc.month, utc.day, utc.hour, utc.minute, utc.second);
            }
            return onvifUTCDateTime;
        }
        /// <summary>
        /// 初始化摄像头服务配置
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>> GetCapabilities(IPEndPoint iPEndPoint)
        {
            string reqMessageStr = @" 
                                      <tds:GetCapabilities> 
                                           <tds:Category>All</tds:Category> 
                                      </tds:GetCapabilities> ";
            var result = await OnvifAuth.RemoteClient(iPEndPoint, URL, reqMessageStr, "user", "pass", DateTime.Now);
            var xnode = result.Html.ReplacePattern(@"(<|/)[a-z\-]+:", "$1");
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return XElement.Parse(xnode).Descendants("XAddr").Select(x => x.Value).ToList();
            }
            else
            {
                return OnvifAuth.ErrorResponse(xnode).ToCast<List<string>>();
            }
        }
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetDeviceInformation(IPEndPoint iPEndPoint, string user, string pass, DateTime onvifUTCDateTime)
        {
            string reqMessageStr = @" 
                                      <tds:GetDeviceInformation /> ";
            var result = await OnvifAuth.RemoteClient(iPEndPoint, URL, reqMessageStr, user, pass, onvifUTCDateTime);
            var xnode = result.Html.ReplacePattern(@"(<|/)[a-z\-]+:", "$1");
            if (result.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    return XElement.Parse(xnode).Descendants("GetDeviceInformationResponse").FirstOrDefault().ToXml().XmlToEntity<XmlValue>().ChildNodes.ToDictionary(x => x.Name, x => x.Value).ToJson();
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
            else
            {
                return OnvifAuth.ErrorResponse(xnode).ToCast<string>();
            }
        }
    }

}
