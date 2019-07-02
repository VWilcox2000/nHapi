using System;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Model.V23.Message;
using NHapi.Model.V23.Segment;
using Xunit;
using Xunit.Abstractions;

namespace NHapi.Tests
{
	public class PipeParsingFixture23
	{
		private readonly ITestOutputHelper _output;

		public PipeParsingFixture23(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public void ParseQRYR02()
		{
			string message = @"MSH|^~\&|CohieCentral|COHIE|Clinical Data Provider|TCH|20060228155525||QRY^R02^QRY_R02|1|P|2.3|
QRD|20060228155525|R|I||||10^RD&Records&0126|38923^^^^^^^^&TCH|||";

			PipeParser parser = new PipeParser();

			IMessage m = parser.Parse(message);

			QRY_R02 qryR02 = m as QRY_R02;

			Assert.NotNull(qryR02);
			Assert.Equal("38923", qryR02.QRD.GetWhoSubjectFilter(0).IDNumber.Value);
		}

		[Fact]
		public void CreateBlankMessage()
		{
			ADT_A01 a01 = new ADT_A01();
			DateTime birthDate = new DateTime(1980, 4, 1);
			a01.MSH.SendingApplication.UniversalID.Value = "ThisOne";
			a01.MSH.ReceivingApplication.UniversalID.Value = "COHIE";
			a01.PID.PatientIDExternalID.ID.Value = "123456";
			a01.PV1.GetAttendingDoctor(0).FamilyName.Value = "Jones";
			a01.PV1.GetAttendingDoctor(0).GivenName.Value = "Mike";
			a01.PID.DateOfBirth.TimeOfAnEvent.SetShortDate(birthDate);

			PipeParser parser = new PipeParser();

			string pipeMessage = parser.Encode(a01);

			Assert.NotNull(pipeMessage);

			IMessage test = parser.Parse(pipeMessage);
			ADT_A01 a01Test = test as ADT_A01;
			Assert.NotNull(a01Test);

			Assert.Equal("COHIE", a01Test.MSH.ReceivingApplication.UniversalID.Value);
			Assert.Equal("123456", a01Test.PID.PatientIDExternalID.ID.Value);

			Assert.Equal(birthDate.ToShortDateString(), a01Test.PID.DateOfBirth.TimeOfAnEvent.GetAsDate().ToShortDateString());

			Assert.Equal("Jones", a01Test.PV1.GetAttendingDoctor(0).FamilyName.Value);
			Assert.Equal("ADT", a01Test.MSH.MessageType.MessageType.Value);
			Assert.Equal("A01", a01Test.MSH.MessageType.TriggerEvent.Value);
		}


		[Fact]
		public void ParseORFR04()
		{
			string message =
				@"MSH|^~\&|Query Result Locator|Query Facility Name|Query Application Name|ST ELSEWHERE HOSPITAL|20051024074506||ORF^R04|432|P|2.3|
MSA|AA|123456789|
QRD|20060228160421|R|I||||10^RD&Records&0126|38923^^^^^^^^&TCH|||
QRF||20050101000000||
PID|||38923^^^ST ELSEWHERE HOSPITAL Medical Record Numbers&              MEDIC              AL RECORD NUMBER&ST ELSEWHERE HOSPITAL^MR^ST ELSEWHERE HOSPITAL||Bombadill^Tom||19450605|M|||1&Main Street^^Littleton^CO^80122||^^^^^303^4376329^22|
OBR|1|0015566|DH2211223|83036^HEMOGLOBIN A1C^^83036^HEMOGLOBIN A1C|||20040526094000|||||||20040526094000||J12345^JENS^JENNY^^^DR^MD^^^^^^^112233&TCH|||||          TP QUEST DIAGNOSTICS-TAMPA 4225 E. FOWLER AVE TAMPA          FL 33617|20030622070400|||F|
OBX|1|NM|50026400^HEMOGLOBIN A1C^^50026400^HEMOGLOBIN A1C||12|^% TOTAL HGB|4.0 - 6.0|H|||F|||20040510094000|TP^^L|";

			PipeParser parser = new PipeParser();

			IMessage m = parser.Parse(message);

			ORF_R04 orfR04 = m as ORF_R04;
			Assert.NotNull(orfR04);
			Assert.Equal("12",
				orfR04.GetQUERY_RESPONSE().GetORDER().GetOBSERVATION().OBX.GetObservationValue()[0].Data.ToString());
		}


		[Fact]
		public void ParseORFR04ToXML()
		{
			string message =
				@"MSH|^~\&|Query Result Locator|Query Facility Name|Query Application Name|ST ELSEWHERE HOSPITAL|20051024074506||ORF^R04|432|P|2.3|
MSA|AA|123456789|
QRD|20060228160421|R|I||||10^RD&Records&0126|38923^^^^^^^^&TCH|||
QRF||20050101000000||
PID|||38923^^^ST ELSEWHERE HOSPITAL Medical Record Numbers&              MEDIC              AL RECORD NUMBER&ST ELSEWHERE HOSPITAL^MR^ST ELSEWHERE HOSPITAL||Bombadill^Tom||19450605|M|||1&Main Street^^Littleton^CO^80122||^^^^^303^4376329^22|
OBR|1|0015566|DH2211223|83036^HEMOGLOBIN A1C^^83036^HEMOGLOBIN A1C|||20040526094000|||||||20040526094000||J12345^JENS^JENNY^^^DR^MD^^^^^^^112233&TCH|||||          TP QUEST DIAGNOSTICS-TAMPA 4225 E. FOWLER AVE TAMPA          FL 33617|20030622070400|||F|
OBX|1|NM|50026400^HEMOGLOBIN A1C^^50026400^HEMOGLOBIN A1C||12|^% TOTAL HGB|4.0 - 6.0|H|||F|||20040510094000|TP^^L|";

			PipeParser parser = new PipeParser();

			IMessage m = parser.Parse(message);

			ORF_R04 orfR04 = m as ORF_R04;

			Assert.NotNull(orfR04);

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(orfR04);

			Assert.NotNull(recoveredMessage);
			Assert.NotEqual(string.Empty, recoveredMessage);
		}

		[Fact]
		public void ParseXMLToHL7()
		{
			string message = GetQRYR02XML();

			XMLParser xmlParser = new DefaultXMLParser();
			IMessage m = xmlParser.Parse(message);

			QRY_R02 qryR02 = m as QRY_R02;

			Assert.NotNull(qryR02);

			PipeParser pipeParser = new PipeParser();

			string pipeOutput = pipeParser.Encode(qryR02);

			Assert.NotNull(pipeOutput);
			Assert.NotEqual(string.Empty, pipeOutput);
		}


		[Fact]
		public void ParseORFR04ToXmlNoOCR()
		{
			string message =
				@"MSH|^~\&|Query Result Locator|Query Facility Name|Query Application Name|ST ELSEWHERE HOSPITAL|20051024074506||ORF^R04|432|P|2.3|
MSA|AA|123456789|
QRD|20060228160421|R|I||||10^RD&Records&0126|38923^^^^^^^^&TCH|||
QRF||20050101000000||
PID|||38923^^^ST ELSEWHERE HOSPITAL Medical Record Numbers&              MEDIC              AL RECORD NUMBER&ST ELSEWHERE HOSPITAL^MR^ST ELSEWHERE HOSPITAL||Bombadill^Tom||19450605|M|||1&Main Street^^Littleton^CO^80122||^^^^^303^4376329^22|
OBR|1|0015566|DH2211223|83036^HEMOGLOBIN A1C^^83036^HEMOGLOBIN A1C|||20040526094000|||||||20040526094000||J12345^JENS^JENNY^^^DR^MD^^^^^^^112233&TCH|||||          TP QUEST DIAGNOSTICS-TAMPA 4225 E. FOWLER AVE TAMPA          FL 33617|20030622070400|||F|
OBX|1|NM|50026400^HEMOGLOBIN A1C^^50026400^HEMOGLOBIN A1C||12|^% TOTAL HGB|4.0 - 6.0|H|||F|||20040510094000|TP^^L|";

			PipeParser parser = new PipeParser();

			IMessage m = parser.Parse(message);

			ORF_R04 orfR04 = m as ORF_R04;

			Assert.NotNull(orfR04);

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(orfR04);

			Assert.NotNull(recoveredMessage);
			Assert.False(recoveredMessage.IndexOf("ORC") > -1, "Returned message added ORC segment.");
		}

		[Fact]
		public void TestOBXDataTypes()
		{
			string message = @"MSH|^~\&|EPIC|AIDI|||20070921152053|ITFCOHIEIN|ORF^R04|297|P|2.3|||
MSA|CA|1
QRD|20060725141358|R|||||10^RD|1130851^^^^MRN|RES|||
QRF|||||||||
OBR|1|5149916^EPC|20050118113415533318^|E8600^ECG^^^ECG|||200501181134||||||Age: 17  yrs ~Criteria: C-HP708 ~|||||1||Zztesttwocorhoi|Results||||F||^^^^^Routine|||||||||200501181134|||||||||
OBX|1|ST|:8601-7^ECG IMPRESSION|2|Normal sinus rhythm, rate  77     Normal P axis, PR, rate & rhythm ||||||F||1|200501181134||
OBX|2|ST|:8625-6^PR INTERVAL|3|141||||||F||1|200501181134||
OBX|3|ST|:8633-0^QRS DURATION|4|83||||||F||1|200501181134||
OBX|4|ST|:8634-8^QT INTERVAL|5|358||||||F||1|200501181134||
OBX|5|ST|:8636-3^QT INTERVAL CORRECTED|6|405||||||F||1|200501181134||
OBX|6|ST|:8626-4^FRONTAL AXIS P|7|-1||||||F||1|200501181134||
OBX|7|ST|:99003^FRONTAL AXIS INITIAL 40 MS|8|41||||||F||1|200501181134||
OBX|8|ST|:8632-2^FRONTAL AXIS MEAN QRS|9|66||||||F||1|200501181134||
OBX|9|ST|:99004^FRONTAL AXIS TERMINAL 40 MS|10|80||||||F||1|200501181134||
OBX|10|ST|:99005^FRONTAL AXIS ST|11|36||||||F||1|200501181134||
OBX|11|ST|:8638-9^FRONTAL AXIS T|12|40||||||F||1|200501181134||
OBX|12|ST|:99006^ECG SEVERITY T|13|- NORMAL ECG - ||||||F||1|200501181134||
OBX|13|DT|5315037^Start Date^Start Collection Dat^ABC||18APR06||||||F|||20060419125100|PPKMG|PPJW^SMITH, Fred 
QAK||OK||1|1|0
";

			PipeParser parser = new PipeParser();

			IMessage m = parser.Parse(message);

			ORF_R04 orfR04 = m as ORF_R04;

			Assert.NotNull(orfR04);

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(orfR04);
		}

		[Fact]
		public void ParseORFR04ToXmlNoNTE()
		{
			string message =
				@"MSH|^~\&|Query Result Locator|Query Facility Name|Query Application Name|ST ELSEWHERE HOSPITAL|20051024074506||ORF^R04|432|P|2.3|
MSA|AA|123456789|
QRD|20060228160421|R|I||||10^RD&Records&0126|38923^^^^^^^^&TCH|||
QRF||20050101000000||
PID|||38923^^^ST ELSEWHERE HOSPITAL Medical Record Numbers&              MEDIC              AL RECORD NUMBER&ST ELSEWHERE HOSPITAL^MR^ST ELSEWHERE HOSPITAL||Bombadill^Tom||19450605|M|||1&Main Street^^Littleton^CO^80122||^^^^^303^4376329^22|
OBR|1|0015566|DH2211223|83036^HEMOGLOBIN A1C^^83036^HEMOGLOBIN A1C|||20040526094000|||||||20040526094000||J12345^JENS^JENNY^^^DR^MD^^^^^^^112233&TCH|||||          TP QUEST DIAGNOSTICS-TAMPA 4225 E. FOWLER AVE TAMPA          FL 33617|20030622070400|||F|
OBX|1|NM|50026400^HEMOGLOBIN A1C^^50026400^HEMOGLOBIN A1C||12|^% TOTAL HGB|4.0 - 6.0|H|||F|||20040510094000|TP^^L|";

			PipeParser parser = new PipeParser();

			IMessage m = parser.Parse(message);

			ORF_R04 orfR04 = m as ORF_R04;

			Assert.NotNull(orfR04);

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(orfR04);

			Assert.NotNull(recoveredMessage);
			Assert.False(recoveredMessage.IndexOf("NTE") > -1, "Returned message added ORC segment.");
		}

		private static string GetQRYR02XML()
		{
			return @"<QRY_R02 xmlns=""urn:hl7-org:v2xml"">
  <MSH>
    <MSH.1>|</MSH.1>
    <MSH.2>^~\&amp;</MSH.2>
    <MSH.1 />
    <MSH.2 />
    <MSH.3>
      <HD.1>CohieCentral</HD.1>
    </MSH.3>
    <MSH.4>
      <HD.1>COHIE</HD.1>
    </MSH.4>
    <MSH.5>
      <HD.1>Clinical Data Provider</HD.1>
    </MSH.5>
    <MSH.6>
      <HD.1>UCH</HD.1>
    </MSH.6>
    <MSH.7>
      <TS.1>20060228152640</TS.1>
    </MSH.7>
    <MSH.9>
      <MSG.1>QRY</MSG.1>
      <MSG.2>R02</MSG.2>
      <MSG.3>QRY_R02</MSG.3>
    </MSH.9>
    <MSH.10>1</MSH.10>
    <MSH.11>
      <PT.1>P</PT.1>
    </MSH.11>
    <MSH.12>
      <VID.1>2.3</VID.1>
    </MSH.12>
  </MSH>
  <QRD>
    <QRD.1>
      <TS.1>20060228152640</TS.1>
    </QRD.1>
    <QRD.2>R</QRD.2>
    <QRD.3>I</QRD.3>
    <QRD.4></QRD.4>
    <QRD.7>
      <CQ.1>10</CQ.1>
      <CQ.2>
        <CE.1>RD</CE.1>
        <CE.2>Records</CE.2>
        <CE.3>0126</CE.3>
      </CQ.2>
    </QRD.7>
    <QRD.8>
      <XCN.1>99388244</XCN.1>
      <XCN.9>
        <HD.2>UCH</HD.2>
      </XCN.9>
    </QRD.8>
    <QRD.9 />
    <QRD.10 />
  </QRD>
  <QRF>
    <QRF.1 />
    <QRF.2>
      <TS.1>20050101000000</TS.1>
    </QRF.2>
    <QRF.3 />
  </QRF>
</QRY_R02>
";
		}

		[Fact]
		public void TestPopulateEVNSegmenValuesGenerically()
		{
			string message = @"MSH|^~\&|SUNS1|OVI02|AZIS|CMD|200606221348||ADT^A01|1049691900|P|2.3
EVN|A01|200601060800
PID||8912716038^^^51276|0216128^^^51276||BARDOUN^LEA SACHA||19981201|F|||AVENUE FRANC GOLD 8^^LUXEMBOURGH^^6780^150||053/12456789||N|S|||99120162652||^^^|||||B
PV1||O|^^|U|||07632^MORTELO^POL^^^DR.|^^^^^|||||N||||||0200001198
PV2|||^^AZIS||N|||200601060800
IN1|0001|2|314000|||||||||19800101|||1|BARDOUN^LEA SACHA|1|19981201|AVENUE FRANC GOLD 8^^LUXEMBOURGH^^6780^150|||||||||||||||||
ZIN|0164652011399|0164652011399|101|101|45789^Broken bone";

			var parser = new PipeParser();
			var abstractMessage = parser.Parse(message);

			// this is the normal / expected way of working with NHapi parsed messages
			var typedMessage = abstractMessage as ADT_A01;
			if (typedMessage != null)
			{
				typedMessage.EVN.OperatorID.FamilyName.Value = "Surname";
				typedMessage.EVN.OperatorID.GivenName.Value = "Firstname";
			}

			var pipeDelimitedMessage = parser.Encode(typedMessage);

			// alternatively, you can apply this modification to any HL7 2.3 message
			// with an EVN segment using this more generic method
			var genericMethod = abstractMessage as AbstractMessage;
			var evn = genericMethod.GetStructure("EVN") as EVN;
			if (evn != null)
			{
				evn.OperatorID.FamilyName.Value = "SurnameGeneric";
				evn.OperatorID.GivenName.Value = "FirstnameGeneric";
			}

			pipeDelimitedMessage = parser.Encode(typedMessage);
		}

		/// <summary>
		/// https://github.com/duaneedwards/nHapi/issues/25
		/// </summary>
		[Fact]
		public void TestGithubIssue25CantGetRepetition()
		{
			string message = @"MSH|^~\&|MILL|EMRY|MQ|EMRY|20150619155451||ADT^A08|Q2043855220T2330403781X928163|P|2.3||||||8859/1
EVN|A08|20150619155451
PID|1|935307^^^EUH MRN^MRN^EH01|25106376^^^TEC MRN~1781893^^^CLH MRN~935307^^^EUH MRN~5938067^^^EMPI|1167766^^^CPI NBR^^EXTERNAL~90509411^^^HNASYSID~10341880^^^HNASYSID~50627780^^^HNASYSID~5938067^^^MSG_CERNPHR|Patient^Test^Test^^^^Cur_Name||19400101|F||WHI|123 ENDOFTHE RD^UNIT 123^ATLANTA^GA^40000^USA^HOME^^||5555555555^HOME~6666666666^YAHOO@YAHOO.COM^EMAIL|6666666666^BUS|ENG|M|OTH|12345665161^^^EUH FIN^FIN NBR^EH01|123454103|GA123450071||Non-Hispanic|||0|""|""|""||N";

			PipeParser parser = new PipeParser();

			IMessage m = parser.Parse(message);

			ADT_A01 adtA01 = m as ADT_A01; // a08 is mapped to a01

			Assert.NotNull(adtA01);

			for (int rep = 0; rep < adtA01.PID.PatientIDInternalIDRepetitionsUsed; rep++)
			{
				var cx = adtA01.PID.GetPatientIDInternalID(rep);
				_output.WriteLine(cx.ID.Value);
			}

			for (int rep = 0; rep < adtA01.PID.AlternatePatientIDRepetitionsUsed; rep++)
			{
				var cx = adtA01.PID.GetAlternatePatientID(rep);
				_output.WriteLine(cx.ID.Value);
			}
		}
	}
}