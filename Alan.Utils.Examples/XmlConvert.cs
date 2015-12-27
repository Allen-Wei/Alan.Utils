using System;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Alan.Utils.ExtensionMethods;

namespace Alan.Utils.Examples
{
    public class XmlConvert
    {
        public static void Run()
        {
            var xml = @"<xml><ToUserName><![CDATA[gh_3209e49c0c17]]></ToUserName>
<FromUserName><![CDATA[oLntxs52aj9suPa5XxqWPKmpB4zs]]></FromUserName>
<CreateTime>1450255418</CreateTime>
<MsgType><![CDATA[text]]></MsgType>
<Content><![CDATA[hello]]></Content>
<MsgId>6228799591557931974</MsgId>
</xml>";
            var model = xml.ExXmlToEntity<Model>();

            var rep = new TextMessageResponse()
            {
                Content = "hello",
                ToUserName = "My name",
                CreateTime = DateTime.Now.ToShortDateString()
            };
            var repXml = model.ExToXml();
        }

        [XmlRoot("xml")]
        public class Model
        {
            [XmlElement("ToUserName")]
            public string ToUserNameAlias { get; set; }

            public string FromUserName { get; set; }
            public string CreateTime { get; set; }
            public string MsgType { get; set; }
            public string Content { get; set; }
            public string MsgId { get; set; }
        }

        /// <summary>
        /// 回复文本消息
        /// </summary>
        [XmlRoot("xml")]
        public class TextMessageResponse
        {
            /// <summary>
            /// 接收方帐号（收到的OpenID）
            /// </summary>
            public string ToUserName { get; set; }

            /// <summary>
            /// 开发者微信号
            /// </summary>
            public string FromUserName { get; set; }

            /// <summary>
            /// 消息创建时间 （整型）
            /// </summary>
            [XmlElement("CreateTimeAlias")]
            public string CreateTime { get; set; }

            public string MsgType
            {
                get { return "text"; }
            }

            /// <summary>
            /// 回复的消息内容（换行：在content中能够换行，微信客户端就支持换行显示）
            /// </summary>
            public string Content { get; set; }
        }
    }
}
