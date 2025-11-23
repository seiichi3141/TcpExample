using System;

namespace TcpExample.Application.Serialization
{
    /// <summary>
    /// XML 出力時に対象要素の直前にコメントを挿入するための属性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class XmlCommentAttribute : Attribute
    {
        public string Text { get; }

        public XmlCommentAttribute(string text)
        {
            Text = text;
        }
    }
}
