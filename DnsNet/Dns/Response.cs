using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Heijden.DNS
{
	public class Response
	{
		/// <summary>
		/// List of Question records
		/// </summary>
		public List<Question> Questions;
		/// <summary>
		/// List of AnswerRR records
		/// </summary>
		public List<AnswerRR> Answers;
		/// <summary>
		/// List of AuthorityRR records
		/// </summary>
		public List<AuthorityRR> Authorities;
		/// <summary>
		/// List of AdditionalRR records
		/// </summary>
		public List<AdditionalRR> Additionals;

		public Header header;

		/// <summary>
		/// Error message, empty when no error
		/// </summary>
		public string Error;

		/// <summary>
		/// The Size of the message
		/// </summary>
		public int MessageSize;

		/// <summary>
		/// TimeStamp when cached
		/// </summary>
		public DateTime TimeStamp;

		/// <summary>
		/// Server which delivered this response
		/// </summary>
		public IPEndPoint Server;

		public Response()
		{
			Questions = new List<Question>();
			Answers = new List<AnswerRR>();
			Authorities = new List<AuthorityRR>();
			Additionals = new List<AdditionalRR>();

			Server = new IPEndPoint(0,0);
			Error = "";
			MessageSize = 0;
			TimeStamp = DateTime.Now;
			header = new Header();
		}

		public Response(IPEndPoint iPEndPoint, byte[] data)
		{
			Error = "";
			Server = iPEndPoint;
			TimeStamp = DateTime.Now;
			MessageSize = data.Length;
			RecordReader rr = new RecordReader(data);

			Questions = new List<Question>();
			Answers = new List<AnswerRR>();
			Authorities = new List<AuthorityRR>();
			Additionals = new List<AdditionalRR>();

			header = new Header(rr);

			//if (header.RCODE != RCode.NoError)
			//	Error = header.RCODE.ToString();

			for (int intI = 0; intI < header.QDCOUNT; intI++)
			{
				Questions.Add(new Question(rr));
			}

			for (int intI = 0; intI < header.ANCOUNT; intI++)
			{
				Answers.Add(new AnswerRR(rr));
			}

			for (int intI = 0; intI < header.NSCOUNT; intI++)
			{
				Authorities.Add(new AuthorityRR(rr));
			}
			for (int intI = 0; intI < header.ARCOUNT; intI++)
			{
				Additionals.Add(new AdditionalRR(rr));
			}
		}

		/// <summary>
		/// List of RecordA in Response.Answers
		/// </summary>
		public RecordA[] RecordsA
		{
			get { return this.Records<RecordA>(); }
		}

		/// <summary>
		/// List of RecordAAAA in Response.Answers
		/// </summary>
		public RecordAAAA[] RecordsAAAA
		{
			get { return this.Records<RecordAAAA>(); }
		}

		/// <summary>
		/// List of RecordPTR in Response.Answers
		/// </summary>
		public RecordPTR[] RecordsPTR
		{
			get { return this.Records<RecordPTR>(); }
		}

		/// <summary>
		/// List of RecordNS in Response.Answers
		/// </summary>
		public RecordNS[] RecordsNS
		{
			get { return this.Records<RecordNS>(); }
		}

		/// <summary>
		/// List of RecordCNAME in Response.Answers
		/// </summary>
		public RecordCNAME[] RecordsCNAME
		{
			get { return this.Records<RecordCNAME>(); }
		}

		/// <summary>
		/// List of RecordMX in Response.Answers
		/// </summary>
		public RecordMX[] RecordsMX
		{
			get { return this.Records<RecordMX>(); }
		}

		/// <summary>
		/// List of RecordTXT in Response.Answers
		/// </summary>
		public RecordTXT[] RecordsTXT
		{
			get { return this.Records<RecordTXT>(); }
		}

		/// <summary>
		/// List of RecordLOC in Response.Answers
		/// </summary>
		public RecordLOC[] RecordsLOC
		{
			get { return this.Records<RecordLOC>(); }
		}

		/// <summary>
		/// List of RecordSRV in Response.Answers
		/// </summary>
		public RecordSRV[] RecordsSRV
		{
			get { return this.Records<RecordSRV>(); }
		}

		/// <summary>
		/// List of Records of the specified type in Response.Answers
		/// </summary>
		/// <typeparam name="T">The type of Record to return</typeparam>
		/// <returns>A list of Records of the specified type</returns>
		public T[] Records<T>() where T : Record
		{
			return this.Answers.AsParallel()
					.Select( rr => rr.RECORD )
					.Where( r => r is T )
					.Cast<T>()
					.ToArray();
		}

		/// <summary>
		/// All Resource Records in Response.Answers, Response.Authorities, and Response.Additionals
		/// </summary>
		public RR[] ResourceRecords
		{
			get
			{
				List<RR> list = new List<RR>();
				list.AddRange(this.Answers);
				list.AddRange(this.Authorities);
				list.AddRange(this.Additionals);
				return list.ToArray();
			}
		}
	}
}
