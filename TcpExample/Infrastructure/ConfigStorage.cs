using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TcpExample.Application.Models;
using TcpExample.Application.Serialization;
using TcpExample.Application.Services;
using System.Reflection;

namespace TcpExample.Infrastructure
{
    /// <summary>
    /// XML 設定ファイルの保存/読込を行う。
    /// </summary>
    public sealed class ConfigStorage : IConfigStorage
    {
        public SettingsModel Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path is not specified.", nameof(path));
            }

            if (!File.Exists(path))
            {
                return CreateDefault();
            }

            var serializer = new XmlSerializer(typeof(SettingsModel));
            using (var stream = File.OpenRead(path))
            {
                return (SettingsModel)serializer.Deserialize(stream);
            }
        }

        public SettingsModel LoadOrDefault(string path)
        {
            try
            {
                return Load(path);
            }
            catch
            {
                return CreateDefault();
            }
        }

        public void Save(string path, SettingsModel config)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path is not specified.", nameof(path));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var serializer = new XmlSerializer(typeof(SettingsModel));

            // Serialize to XDocument so we can add comments and control encoding/indentation.
            XDocument doc;
            using (var ms = new MemoryStream())
            {
                serializer.Serialize(ms, config);
                ms.Position = 0;
                doc = XDocument.Load(ms);
            }

            ApplyComments(doc.Root, typeof(SettingsModel));

            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                Indent = true,
                OmitXmlDeclaration = false,
                NewLineOnAttributes = false
            };

            using (var writer = XmlWriter.Create(path, settings))
            {
                doc.Save(writer);
            }
        }

        private static SettingsModel CreateDefault()
        {
            var settings = new SettingsModel();
            Application.Services.DefaultValueApplier.Apply(settings);
            EnsureDefaults(settings);
            return settings;
        }

        private static void ApplyComments(XElement root, Type type)
        {
            if (root == null || type == null)
            {
                return;
            }

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var commentAttr = prop.GetCustomAttribute<XmlCommentAttribute>();
                if (commentAttr == null)
                {
                    continue;
                }

                // Determine element name
                var elementName = prop.Name;
                var xmlElement = prop.GetCustomAttribute<XmlElementAttribute>();
                if (xmlElement != null && !string.IsNullOrWhiteSpace(xmlElement.ElementName))
                {
                    elementName = xmlElement.ElementName;
                }
                var xmlArray = prop.GetCustomAttribute<XmlArrayAttribute>();
                if (xmlArray != null && !string.IsNullOrWhiteSpace(xmlArray.ElementName))
                {
                    elementName = xmlArray.ElementName;
                }

                var targetElement = root.Element(elementName);
                if (targetElement != null)
                {
                    targetElement.AddBeforeSelf(new XComment(commentAttr.Text));

                    // Recurse into child
                    ApplyComments(targetElement, prop.PropertyType);
                }
            }
        }

        private static void EnsureDefaults(SettingsModel settings)
        {
            if (settings == null)
            {
                return;
            }

            if (settings.Connection1 == null)
            {
                settings.Connection1 = new ConnectionSettingsModel { EndpointIp = "127.0.0.1", Port = 9000 };
            }
            else
            {
                if (string.IsNullOrWhiteSpace(settings.Connection1.EndpointIp))
                {
                    settings.Connection1.EndpointIp = "127.0.0.1";
                }
                if (settings.Connection1.Port <= 0)
                {
                    settings.Connection1.Port = 9000;
                }
            }
            if (settings.Connection2 == null)
            {
                settings.Connection2 = new ConnectionSettingsModel { EndpointIp = "127.0.0.1", Port = 9000 };
            }
            else
            {
                if (string.IsNullOrWhiteSpace(settings.Connection2.EndpointIp))
                {
                    settings.Connection2.EndpointIp = "127.0.0.1";
                }
                if (settings.Connection2.Port <= 0)
                {
                    settings.Connection2.Port = 9000;
                }
            }

            if (settings.AutoResponse == null)
            {
                settings.AutoResponse = new AutoResponseSettingsModel();
            }

            if (settings.AutoResponse.Rules == null || settings.AutoResponse.Rules.Count == 0)
            {
                settings.AutoResponse.Rules = new List<AutoResponseRuleModel>
                {
                    new AutoResponseRuleModel
                    {
                        Name = "PingPong",
                        Pattern = "PING",
                        Response = "PONG",
                        Enabled = true,
                        Priority = 1
                    }
                };
            }
        }
    }
}
