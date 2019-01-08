﻿using System.Collections.Generic;
using FatturaElettronica.Defaults;
using FatturaElettronica.Common;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Xml;
using System.IO;
using System.Text;
using System;

namespace FatturaElettronica
{
    public class Fattura : BaseClassSerializable
    {
        public Fattura()
        {
            FatturaElettronicaHeader = new FatturaElettronicaHeader.FatturaElettronicaHeader();
            FatturaElettronicaBody = new List<FatturaElettronicaBody.FatturaElettronicaBody>();
        }

        public override void WriteXml(System.Xml.XmlWriter w)
        {
            w.WriteStartElement(RootElement.Prefix, RootElement.LocalName, RootElement.NameSpace);
            w.WriteAttributeString("versione", FatturaElettronicaHeader.DatiTrasmissione.FormatoTrasmissione);
            foreach (var a in RootElement.ExtraAttributes)
            {
                w.WriteAttributeString(a.Prefix, a.LocalName, a.ns, a.value);
            }
            base.WriteXml(w);
            w.WriteEndElement();
        }

        public override void ReadXml(System.Xml.XmlReader r)
        {
            r.MoveToContent();
            base.ReadXml(r);
        }

        public void ReadXml(string xml)
        {
            var preambleByte = Encoding.UTF8.GetPreamble();
            string _byteOrderMarkUtf8 = System.Text.Encoding.UTF8.GetString(preambleByte, 0, preambleByte.Length);
            if (xml.StartsWith(_byteOrderMarkUtf8, StringComparison.Ordinal))
            {
                int index = xml.IndexOf('<');
                if (index > 0)
                {
                    xml = xml.Substring(index, xml.Length - index);
                }
            }

            var r = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { IgnoreWhitespace = true, IgnoreComments = true });

            ReadXml(r);
        }

        public static Fattura CreateInstance(Instance formato)
        {
            var f = new Fattura();

            switch (formato)
            {
                case Instance.PubblicaAmministrazione:
                    f.FatturaElettronicaHeader.DatiTrasmissione.FormatoTrasmissione = FormatoTrasmissione.PubblicaAmministrazione;
                    break;
                case Instance.Privati:
                    f.FatturaElettronicaHeader.DatiTrasmissione.FormatoTrasmissione = FormatoTrasmissione.Privati;
                    f.FatturaElettronicaHeader.DatiTrasmissione.CodiceDestinatario = new string('0', 7);
                    break;
            }

            return f;
        }

        /// IMPORTANT
        /// Each data property must be flagged with the Order attribute or it will be ignored.
        /// Also, properties must be listed with the precise order in the specification.

        /// <summary>
        /// Intestazione della comunicazione.
        /// </summary>
        [DataProperty]
        public FatturaElettronicaHeader.FatturaElettronicaHeader FatturaElettronicaHeader { get; set; }

        /// <summary>
        /// Lotto di fatture incluse nella comunicazione.
        /// </summary>
        /// <remarks>Il blocco ha molteciplità 1 nel caso di fattura singola; nel caso di lotto di fatture, si ripete
        /// per ogni fattura componente il lotto stesso.</remarks>
        [DataProperty]
        public List<FatturaElettronicaBody.FatturaElettronicaBody> FatturaElettronicaBody { get; set; }

    }
}
