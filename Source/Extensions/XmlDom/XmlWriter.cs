using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using PHP.Core;
using System.Runtime.InteropServices;
using System.IO;

namespace PHP.Library.Xml
{
    [ImplementsType]
    public partial class XMLWriter
    {
        XmlTextWriter _writer;
		MemoryStream _buffer;
		Encoding _encoding;
        
        [PhpVisible]
        public bool openMemory()
        {
			_buffer = new MemoryStream();
            return true;
        }
        
        [PhpVisible]
        public bool setIndent(bool indent)
        {
			_writer.Indentation = 2;
            return true;
        }

        [PhpVisible]
        public bool startDocument([Optional] [Nullable] string version, [Optional] [Nullable] string encoding, [Optional] [Nullable] string standalone)
        {
			_encoding = Encoding.GetEncoding(encoding);
			_writer = new XmlTextWriter(_buffer, _encoding);
            bool? bStandalone = null;
            if (standalone == "yes")
                bStandalone = true;
            else if (standalone == "no")
                bStandalone = false;
            if (bStandalone.HasValue)
                _writer.WriteStartDocument(bStandalone.Value);
            else
				_writer.WriteStartDocument();
            return true;
        }

        [PhpVisible]
        public bool startElement(string name)
        {
            _writer.WriteStartElement(name);
            return true;
        }

        [PhpVisible]
        public bool writeAttribute ( string name , string value )
        {
            _writer.WriteAttributeString(name, value);
            return true;
        }

        [PhpVisible]
        public bool endElement()
        {
            _writer.WriteEndElement();
            return true;
        }

        [PhpVisible]
        public bool writeElement(string name, [Optional] [Nullable] object content)
        {
			//if (content == null)
			//{
			//	_writer.WriteElementString(name, content.ToString());
			//	return true;
			//}

            startElement(name);

			if (content != null)
            if (content is string)
                _writer.WriteString((string)content);
            else
                _writer.WriteValue(content);


            endElement();
            return true;
        }

        [PhpVisible]
		public bool text(string content)
		{
        	_writer.WriteString(content);
			return true;
		}

        [PhpVisible]
        public string outputMemory([Optional] [Nullable] bool flush)
        {
            if (flush)
                _writer.Flush();
			
			_buffer.Position = 0;
			var sr = new StreamReader(_buffer, _encoding);
			var myStr = sr.ReadToEnd();

			//效果同iconv
			var fromEncoding = Encoding.GetEncoding("GBK");
			var toEncoding = Encoding.UTF8;
			var fromBytes = fromEncoding.GetBytes(myStr);
			var toBytes = Encoding.Convert(fromEncoding, toEncoding, fromBytes);
			return Encoding.GetEncoding("ISO-8859-1").GetString(toBytes);
		}
    }
}
