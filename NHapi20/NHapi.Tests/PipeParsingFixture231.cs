using System.Xml;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Model.V231.Message;
using Xunit;

namespace NHapi.Tests
{
	
	public class PipeParsingFixture231
	{
		[Fact]
		public void ParseQRYR02()
		{
			string Message =
				"MSH|^~\\&|CohieCentral|COHIE|Clinical Data Provider|TCH|20060228155525||QRY^R02^QRY_R02|1|P|2.3.1|\r\n" +
				"QRD|20060228155525|R|I||||10^RD&Records&0126|38923^^^^^^^^&TCH|||";

			PipeParser Parser = new PipeParser();

			IMessage m = Parser.Parse(Message);

			QRY_R02 qryR02 = m as QRY_R02;

			Assert.NotNull(qryR02);
			Assert.Equal("38923", qryR02.QRD.GetWhoSubjectFilter(0).IDNumber.Value);
		}

		[Fact]
		public void ParseORMo01PIDSegment()
		{
			string Message = "MSH|^~\\&|INVISION|DHC|SUNQUEST LAB||200606191615||ORM^O01|ORDR|P|2.3.1|LAB\r\n" +
			                 "PID|0001||3020956||TRAINONLYPOE^ONE||19770903|F||W||||||||40230443\r\n" +
			                 "PV1|0001|I|MICU^W276^01||||045716^ABAZA, MONA M|||MED|||||||045716|F|000000030188\r\n" +
			                 "ORC|NW|01444^00001|||||||||||L\r\n" +
			                 "OBR||01444^00001||CAI^CALCIUM IONIZED|||200606191614||||L|||||045716^STEELE, ANDREW W|||||||||||00001&UNITS^ONCE&ONCE^000^200606191614^200606191614^ROUTINE\r\n";

			PipeParser Parser = new PipeParser();

			IMessage m = Parser.Parse(Message);

			ORM_O01 ormo01 = m as ORM_O01;

			Assert.NotNull(ormo01);
			Assert.Equal("TRAINONLYPOE", (ormo01.PATIENT.PID.GetPatientName()[0]).FamilyLastName.FamilyName.Value);
			Assert.Equal("19770903", ormo01.PATIENT.PID.DateTimeOfBirth.TimeOfAnEvent.Value);
			Assert.Equal("F", ormo01.PATIENT.PID.Sex.Value);
			Assert.Equal("W", ((ormo01.PATIENT.PID.GetRace()[0]).Identifier.Value));

			Assert.Equal("ABAZA, MONA M",
				ormo01.PATIENT.PATIENT_VISIT.PV1.GetAttendingDoctor(0).FamilyLastName.FamilyName.Value);
		}

		[Fact]
		public void ParseORMo01ToXml()
		{
			string Message = "MSH|^~\\&|INVISION|DHC|SUNQUEST LAB||200606191615||ORM^O01|ORDR|P|2.3.1|LAB\r\n" +
			                 "PID|0001||3020956||TRAINONLYPOE^ONE||19770903|F||W||||||||40230443\r\n" +
			                 "PV1|0001|I|MICU^W276^01||||045716^ABAZA, MONA M|||MED|||||||045716|F|000000030188\r\n" +
			                 "ORC|NW|01444^00001|||||||||||L\r\n" +
			                 "OBR||01444^00001||CAI^CALCIUM IONIZED|||200606191614||||L|||||045716^STEELE, ANDREW W|||||||||||00001&UNITS^ONCE&ONCE^000^200606191614^200606191614^ROUTINE\r\n";

			PipeParser Parser = new PipeParser();

			IMessage m = Parser.Parse(Message);

			ORM_O01 ormo01 = m as ORM_O01;

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(ormo01);

			Assert.NotEqual(string.Empty, recoveredMessage);

			XmlDocument ormDoc = new XmlDocument();
			ormDoc.LoadXml(recoveredMessage);
			Assert.NotNull(ormDoc);
		}

		[Fact]
		public void ParseORRo02ToXml()
		{
			string Message = "MSH|^~\\&|INVISION|DHC|SUNQUEST LAB||200607100719||ORR^O02|ORDR|T|2.3.1|LAB\r\n" +
			                 "PID|0001||3017864||HILBERT^MARY||19440823|F||W||||||||40244246\r\n" +
			                 "PV1|0001|O|LW||||888883^DOCTOR, UNASSIGNED||||||||||888883|O|000000031540\r\n" +
			                 "ORC|NA|00003^00001|F1492|||||||||888883\r\n" +
			                 "OBR||00003^00001|F1492|RESPC^CULTURE RESPIRATORY ROUTINE|||||||L|||||||||F1492|||||||^ONCE&ONCE^^200607070600^200607070600^ROUTINE\r\n";

			PipeParser Parser = new PipeParser();

			IMessage m = Parser.Parse(Message);

			ORR_O02 msg = m as ORR_O02;

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(msg);

			Assert.NotEqual(string.Empty, recoveredMessage);

			XmlDocument orrDoc = new XmlDocument();
			orrDoc.LoadXml(recoveredMessage);
			Assert.NotNull(orrDoc);
		}


		[Fact]
		public void ParseORUr01LongToXml()
		{
			string Message = "MSH|^~\\$|LAB|DHC|LCR|DH|200511291403||ORU^R01|52780002432|P|2.3.1\r\n" +
			                 "PID|0001|3013839|40206609||BARNES^TEST||19551005|F|||||||||||258452152\r\n" +
			                 "OBR||00009^001|W442|CBC^CBC|||200509210520||||||||CBC^CBC|117564^STEEL||||||||DAH\r\n" +
			                 "OBX|1|NM|WBC||20.0|k/uL|4.5-10.0|H|||Z\r\n" +
			                 "OBX|1|TX|WBC|1|(Ref Range: 4 k/uL)|k/uL|4.5-10.0||||Z\r\n" +
			                 "OBX|2|NM|RBC||4.00|M/uL|4.20-5.40|L|||Z\r\n" +
			                 "OBX|2|TX|RBC|1|(Ref Range: 4 M/uL)|M/uL|4.20-5.40||||Z\r\n" +
			                 "OBX|3|NM|HGB||12.0|g/dL|14.0-24.0|L|||Z\r\n" +
			                 "OBX|3|TX|HGB|1|(Ref Range: 1 g/dL)|g/dL|14.0-24.0||||Z\r\n" +
			                 "OBX|4|NM|HCT||41.0|%|37.0-47.0||||Z\r\n" +
			                 "OBX|4|TX|HCT|1|(Ref Range: 3 %)|%|37.0-47.0||||Z\r\n" +
			                 "OBX|5|NM|MCV||80.9|fl|80.0-100.0||||Z\r\n" +
			                 "OBX|5|TX|MCV|1|(Ref Range: 8 fl)|fl|80.0-100.0||||Z\r\n" +
			                 "OBX|6|NM|MCH||31.0|pg|27.0-31.0||||Z\r\n" +
			                 "OBX|6|TX|MCH|1|(Ref Range: 2 pg)|pg|27.0-31.0||||Z\r\n" +
			                 "OBX|7|NM|MCHC||32.0|g/dL|32.0-36.0||||Z\r\n" +
			                 "OBX|7|TX|MCHC|1|(Ref Range: 3 g/dL)|g/dL|32.0-36.0||||Z\r\n" +
			                 "OBX|8|NM|RDW||19.0|%|11.5-14.5|H|||Z\r\n" +
			                 "OBX|8|TX|RDW|1|(Ref Range: 1 %)|%|11.5-14.5||||Z\r\n" +
			                 "OBX|9|NM|PLTC||45|k/uL|150-400|PL^Y|||Z\r\n" +
			                 "OBX|9|TX|PLTC|1|(Ref Range: 1 k/uL)|k/uL|150-400||||Z\r\n" +
			                 "OBX|10|NM|MPV||10.0|fL|6.2-10.0||||Z\r\n" +
			                 "OBX|10|TX|MPV|1|(Ref Range: 6 fL)|fL|6.2-10.0||||Z\r\n";

			PipeParser Parser = new PipeParser();

			IMessage m = Parser.Parse(Message);

			ORU_R01 msg = m as ORU_R01;

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(msg);

			Assert.NotEqual(string.Empty, recoveredMessage);

			XmlDocument orrDoc = new XmlDocument();
			orrDoc.LoadXml(recoveredMessage);
			Assert.NotNull(orrDoc);
		}

		[Fact]
		public void ParseORFR04()
		{
			string Message =
				"MSH|^~\\&|Query Result Locator|Query Facility Name|Query Application Name|ST ELSEWHERE HOSPITAL|20051024074506||ORF^R04|432|P|2.3.1|\r\n" +
				"MSA|AA|123456789|\r\n" +
				"QRD|20060228160421|R|I||||10^RD&Records&0126|38923^^^^^^^^&TCH|||\r\n" +
				"QRF||20050101000000||\r\n" +
				"PID|||38923^^^ST ELSEWHERE HOSPITAL Medical Record Numbers&              MEDIC              AL RECORD NUMBER&ST ELSEWHERE HOSPITAL^MR^ST ELSEWHERE HOSPITAL||Bombadill^Tom||19450605|M|||1&Main Street^^Littleton^CO^80122||^^^^^303^4376329^22|\r\n" +
				"OBR|1|0015566|DH2211223|83036^HEMOGLOBIN A1C^^83036^HEMOGLOBIN A1C|||20040526094000|||||||20040526094000||J12345^JENS^JENNY^^^DR^MD^^^^^^^112233&TCH|||||          TP QUEST DIAGNOSTICS-TAMPA 4225 E. FOWLER AVE TAMPA          FL 33617|20030622070400|||F|\r\n" +
				"OBX|1|NM|50026400^HEMOGLOBIN A1C^^50026400^HEMOGLOBIN A1C||12|^% TOTAL HGB|4.0 - 6.0|H|||F|||20040510094000|TP^^L|";

			PipeParser Parser = new PipeParser();

			IMessage m = Parser.Parse(Message);

			ORF_R04 orfR04 = m as ORF_R04;
			Assert.NotNull(orfR04);
			Assert.Equal("12",
				orfR04.GetQUERY_RESPONSE().GetORDER().GetOBSERVATION().OBX.GetObservationValue()[0].Data.ToString());
		}

		[Fact]
		public void ParseORFR04ToXML()
		{
			string Message =
				"MSH|^~\\&|Query Result Locator|Query Facility Name|Query Application Name|ST ELSEWHERE HOSPITAL|20051024074506||ORF^R04|432|P|2.3.1|\r\n" +
				"MSA|AA|123456789|\r\n" +
				"QRD|20060228160421|R|I||||10^RD&Records&0126|38923^^^^^^^^&TCH|||\r\n" +
				"QRF||20050101000000||\r\n" +
				"PID|||38923^^^ST ELSEWHERE HOSPITAL Medical Record Numbers&              MEDIC              AL RECORD NUMBER&ST ELSEWHERE HOSPITAL^MR^ST ELSEWHERE HOSPITAL||Bombadill^Tom||19450605|M|||1&Main Street^^Littleton^CO^80122||^^^^^303^4376329^22|\r\n" +
				"OBR|1|0015566|DH2211223|83036^HEMOGLOBIN A1C^^83036^HEMOGLOBIN A1C|||20040526094000|||||||20040526094000||J12345^JENS^JENNY^^^DR^MD^^^^^^^112233&TCH|||||          TP QUEST DIAGNOSTICS-TAMPA 4225 E. FOWLER AVE TAMPA          FL 33617|20030622070400|||F|\r\n" +
				"OBX|1|NM|50026400^HEMOGLOBIN A1C^^50026400^HEMOGLOBIN A1C||12|^% TOTAL HGB|4.0 - 6.0|H|||F|||20040510094000|TP^^L|";

			PipeParser Parser = new PipeParser();

			IMessage m = Parser.Parse(Message);

			ORF_R04 orfR04 = m as ORF_R04;

			Assert.NotNull(orfR04);

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(orfR04);

			Assert.NotNull(recoveredMessage);
			Assert.NotEqual(string.Empty, recoveredMessage);
		}

		/// <summary>
		/// translate a more complex ORM Message
		/// </summary>
		[Fact]
		public void ParseORMwithOBXToXML()
		{
			string Message = "MSH|^~\\&|INVISION|DHC|SUNQUEST LAB||200606191615||ORM^O01|ORDR|P|2.3.1|LAB\r\n" +
			                 "PID|0001||3020956||TRAINONLYPOE^ONE||19770903|F||W||||||||40230443\r\n" +
			                 "PV1|0001|I|MICU^W276^01||||045716^ABAZA, MONA M|||MED|||||||045716|F|000000030188\r\n" +
			                 "ORC|NW|01444^00001|||||||||||L\r\n" +
			                 "OBR||01444^00001||CAI^CALCIUM IONIZED|||200606191614||||L|||||045716^STEELE, ANDREW W|||||||||||00001&UNITS^ONCE&ONCE^000^200606191614^200606191614^ROUTINE\r\n" +
			                 "OBX||NM|||999||||||\r\n" +
			                 "OBX||NM|||999||||||\r\n" +
			                 "OBX||NM|||999||||||";

			PipeParser Parser = new PipeParser();

			IMessage m = Parser.Parse(Message);

			ORM_O01 msgObj = m as ORM_O01;

			Assert.NotNull(msgObj);

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(msgObj);

			Assert.NotNull(recoveredMessage);
			Assert.NotEqual(string.Empty, recoveredMessage);
		}

		/// <summary>
		/// translate a more complex ORM Message
		/// </summary>
		[Fact]
		public void ParseORMwithCompleteOBXToXML()
		{
			string Message = "MSH|^~\\&|INVISION|DHC|SUNQUEST LAB||200606191615||ORM^O01|ORDR|P|2.3.1|LAB\r\n" +
			                 "PID|0001||3020956||TRAINONLYPOE^ONE||19770903|F||W||||||||40230443\r\n" +
			                 "PV1|0001|I|MICU^W276^01||||045716^ABAZA, MONA M|||MED|||||||045716|F|000000030188\r\n" +
			                 "ORC|NW|01444^00001|||||||||||L\r\n" +
			                 "OBR||01444^00001||CAI^CALCIUM IONIZED|||200606191614||||L|||||045716^STEELE, ANDREW W|||||||||||00001&UNITS^ONCE&ONCE^000^200606191614^200606191614^ROUTINE\r\n" +
			                 "OBX|1|TX|SDES||Blood, peripheral||||||Z\r\n" +
			                 "OBX|2|TX|SREQ||LEFT ANTECUBITAL||||||Z\r\n" +
			                 "OBX|3|TX|CULT||Beta hemolytic Streptococcus Group A||||||Z\r\n" +
			                 "OBX|4|TX|CULT||Critical result(s) called to and verification ";
			Message += "\"read-back\"";
			Message += @" received from: Nu~||||||Z
";

			PipeParser Parser = new PipeParser();

			IMessage m = Parser.Parse(Message);

			ORM_O01 msgObj = m as ORM_O01;

			Assert.NotNull(msgObj);

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(msgObj);

			Assert.NotNull(recoveredMessage);
			Assert.NotEqual(string.Empty, recoveredMessage);
		}

		[Fact]
		public void ParseXMLToHL7()
		{
			string Message = GetQRYR02XML();

			XMLParser xmlParser = new DefaultXMLParser();
			IMessage m = xmlParser.Parse(Message);

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
			string Message =
				"MSH|^~\\&|Query Result Locator|Query Facility Name|Query Application Name|ST ELSEWHERE HOSPITAL|20051024074506||ORF^R04|432|P|2.3.1|\r\n" +
				"MSA|AA|123456789|\r\n" +
				"QRD|20060228160421|R|I||||10^RD&Records&0126|38923^^^^^^^^&TCH|||\r\n" +
				"QRF||20050101000000||\r\n" +
				"PID|||38923^^^ST ELSEWHERE HOSPITAL Medical Record Numbers&              MEDIC              AL RECORD NUMBER&ST ELSEWHERE HOSPITAL^MR^ST ELSEWHERE HOSPITAL||Bombadill^Tom||19450605|M|||1&Main Street^^Littleton^CO^80122||^^^^^303^4376329^22|\r\n" +
				"OBR|1|0015566|DH2211223|83036^HEMOGLOBIN A1C^^83036^HEMOGLOBIN A1C|||20040526094000|||||||20040526094000||J12345^JENS^JENNY^^^DR^MD^^^^^^^112233&TCH|||||          TP QUEST DIAGNOSTICS-TAMPA 4225 E. FOWLER AVE TAMPA          FL 33617|20030622070400|||F|\r\n" +
				"OBX|1|NM|50026400^HEMOGLOBIN A1C^^50026400^HEMOGLOBIN A1C||12|^% TOTAL HGB|4.0 - 6.0|H|||F|||20040510094000|TP^^L|";

			PipeParser Parser = new PipeParser();

			IMessage m = Parser.Parse(Message);

			ORF_R04 orfR04 = m as ORF_R04;

			Assert.NotNull(orfR04);

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(orfR04);

			Assert.NotNull(recoveredMessage);
			Assert.False(recoveredMessage.IndexOf("ORC") > -1, "Returned Message added ORC segment.");
		}

		[Fact]
		public void ParseORFR04ToXmlNoNTE()
		{
			string Message =
				"MSH|^~\\&|Query Result Locator|Query Facility Name|Query Application Name|ST ELSEWHERE HOSPITAL|20051024074506||ORF^R04|432|P|2.3.1|\r\n" +
				"MSA|AA|123456789|\r\n" +
				"QRD|20060228160421|R|I||||10^RD&Records&0126|38923^^^^^^^^&TCH|||\r\n" +
				"QRF||20050101000000||\r\n" +
				"PID|||38923^^^ST ELSEWHERE HOSPITAL Medical Record Numbers&              MEDIC              AL RECORD NUMBER&ST ELSEWHERE HOSPITAL^MR^ST ELSEWHERE HOSPITAL||Bombadill^Tom||19450605|M|||1&Main Street^^Littleton^CO^80122||^^^^^303^4376329^22|\r\n" +
				"OBR|1|0015566|DH2211223|83036^HEMOGLOBIN A1C^^83036^HEMOGLOBIN A1C|||20040526094000|||||||20040526094000||J12345^JENS^JENNY^^^DR^MD^^^^^^^112233&TCH|||||          TP QUEST DIAGNOSTICS-TAMPA 4225 E. FOWLER AVE TAMPA          FL 33617|20030622070400|||F|\r\n" +
				"OBX|1|NM|50026400^HEMOGLOBIN A1C^^50026400^HEMOGLOBIN A1C||12|^% TOTAL HGB|4.0 - 6.0|H|||F|||20040510094000|TP^^L|";

			PipeParser Parser = new PipeParser();

			IMessage m = Parser.Parse(Message);

			ORF_R04 orfR04 = m as ORF_R04;

			Assert.NotNull(orfR04);

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(orfR04);

			Assert.NotNull(recoveredMessage);
			Assert.False(recoveredMessage.IndexOf("NTE") > -1, "Returned Message added ORC segment.");
		}

		[Fact]
		public void ParseORFR04FromDHTest()
		{
			string Message =
				"MSH|^~\\&|Clinical Data Provider|DHHA|COHIECentral|COHIE|200609221408||ORF^R04||P|2.3.1\r\n" +
				"MSA|AA|\r\n" +
				"PID|2019877||2019877^^^DH^MR||LOPEZ1^JAMES^TRISTAN||19740804|M\r\n" +
				"OBR||00677^001|M428|CBC^CBC|||200511071505||||||||CBC^CBC|045716^STEELE||||||||DAH\r\n" +
				"OBX|1|NM|WBC||1.1|k/uL|5.0-16.0|L|||C\r\n" +
				"OBX|1|TX|WBC|1|(Ref Range: 5 k/uL)|k/uL|5.0-16.0||||C\r\n" +
				"OBX|1|TX|WBC||Result(s) called to and verification read-back received from:  NURSE NAN M~||||||C\r\n" +
				"OBX|1|TX|WBC||ICU 13:39 11  19  05||||||C\r\n" +
				"OBX|1|TX|WBC||Corrected on 11/29 AT 1337: Previously reported as: 1.1||||||C\r\n" +
				"OBX|2|NM|RBC||3.99|M/uL|3.80-5.40||||Z\r\n" +
				"OBX|2|TX|RBC|1|(Ref Range: 3 M/uL)|M/uL|3.80-5.40||||Z\r\n" +
				"OBX|3|NM|HGB||13.0|g/dL|14.0-24.0|L|||Z\r\n" +
				"OBX|3|TX|HGB|1|(Ref Range: 1 g/dL)|g/dL|14.0-24.0||||Z\r\n" +
				"OBX|4|NM|HCT||40.0|%|31.0-43.0||||Z\r\n" +
				"OBX|4|TX|HCT|1|(Ref Range: 3 %)|%|31.0-43.0||||Z\r\n" +
				"OBX|5|NM|MCV||80.0|fl|70.0-90.0||||Z\r\n" +
				"OBX|5|TX|MCV|1|(Ref Range: 7 fl)|fl|70.0-90.0||||Z\r\n" +
				"OBX|6|NM|MCH||32.0|pg|27.0-31.0|H|||Z\r\n" +
				"OBX|6|TX|MCH|1|(Ref Range: 2 pg)|pg|27.0-31.0||||Z\r\n" +
				"OBX|7|NM|MCHC||32.0|g/dL|32.0-36.0||||Z\r\n" +
				"OBX|7|TX|MCHC|1|(Ref Range: 3 g/dL)|g/dL|32.0-36.0||||Z\r\n" +
				"OBX|8|NM|RDW||17.0|%|11.5-14.5|H|||Z\r\n" +
				"OBX|8|TX|RDW|1|(Ref Range: 1 %)|%|11.5-14.5||||Z\r\n" +
				"OBX|9|NM|PLTC||2|k/uL|150-400|PL^Y|||Z\r\n" +
				"OBX|9|TX|PLTC|1|(Ref Range: 1 k/uL)|k/uL|150-400||||Z\r\n" +
				"OBX|10|NM|MPV||10.0|fL|6.2-10.0||||Z\r\n" +
				"OBX|10|TX|MPV|1|(Ref Range: 6 fL)|fL|6.2-10.0||||Z\r\n" +
				"OBR||00677^001|M428|CBC^CBC|||200511071505||||||||CBC^CBC|045716^STEELE||||||||DAH\r\n" +
				"OBX|1|NM|WBC||1.1|k/uL|5.0-16.0|L|||C\r\n" +
				"OBX|1|TX|WBC|1|(Ref Range: 5 k/uL)|k/uL|5.0-16.0||||C\r\n" +
				"OBX|1|TX|WBC||Result(s) called to and verification read-back received from:  NURSE NAN M~||||||C\r\n" +
				"OBX|1|TX|WBC||ICU 13:39 11  19  05||||||C\r\n" +
				"OBX|1|TX|WBC||Corrected on 11/29 AT 1337: Previously reported as: 1.1||||||C\r\n" +
				"OBX|2|NM|RBC||3.99|M/uL|3.80-5.40||||Z\r\n" +
				"OBX|2|TX|RBC|1|(Ref Range: 3 M/uL)|M/uL|3.80-5.40||||Z\r\n" +
				"OBX|3|NM|HGB||13.0|g/dL|14.0-24.0|L|||Z\r\n" +
				"OBX|3|TX|HGB|1|(Ref Range: 1 g/dL)|g/dL|14.0-24.0||||Z\r\n" +
				"OBX|4|NM|HCT||40.0|%|31.0-43.0||||Z\r\n" +
				"OBX|4|TX|HCT|1|(Ref Range: 3 %)|%|31.0-43.0||||Z\r\n" +
				"OBX|5|NM|MCV||80.0|fl|70.0-90.0||||Z\r\n" +
				"OBX|5|TX|MCV|1|(Ref Range: 7 fl)|fl|70.0-90.0||||Z\r\n" +
				"OBX|6|NM|MCH||32.0|pg|27.0-31.0|H|||Z\r\n" +
				"OBX|6|TX|MCH|1|(Ref Range: 2 pg)|pg|27.0-31.0||||Z\r\n" +
				"OBX|7|NM|MCHC||32.0|g/dL|32.0-36.0||||Z\r\n" +
				"OBX|7|TX|MCHC|1|(Ref Range: 3 g/dL)|g/dL|32.0-36.0||||Z\r\n" +
				"OBX|8|NM|RDW||17.0|%|11.5-14.5|H|||Z\r\n" +
				"OBX|8|TX|RDW|1|(Ref Range: 1 %)|%|11.5-14.5||||Z\r\n" +
				"OBX|9|NM|PLTC||2|k/uL|150-400|PL^Y|||Z\r\n" +
				"OBX|9|TX|PLTC|1|(Ref Range: 1 k/uL)|k/uL|150-400||||Z\r\n" +
				"OBX|10|NM|MPV||10.0|fL|6.2-10.0||||Z\r\n" +
				"OBX|10|TX|MPV|1|(Ref Range: 6 fL)|fL|6.2-10.0||||Z\r\n" +
				"OBR||00677^001|M428|CBC^CBC|||200511071505||||||||CBC^CBC|045716^STEELE||||||||DAH\r\n" +
				"OBX|1|NM|WBC||1.1|k/uL|5.0-16.0|L|||C\r\n" +
				"OBX|1|TX|WBC|1|(Ref Range: 5 k/uL)|k/uL|5.0-16.0||||C\r\n" +
				"OBX|1|TX|WBC||Result(s) called to and verification read-back received from:  NURSE NAN M~||||||C\r\n" +
				"OBX|1|TX|WBC||ICU 13:39 11  19  05||||||C\r\n" +
				"OBX|1|TX|WBC||Corrected on 11/29 AT 1337: Previously reported as: 1.1||||||C\r\n" +
				"OBX|2|NM|RBC||3.99|M/uL|3.80-5.40||||Z\r\n" +
				"OBX|2|TX|RBC|1|(Ref Range: 3 M/uL)|M/uL|3.80-5.40||||Z\r\n" +
				"OBX|3|NM|HGB||13.0|g/dL|14.0-24.0|L|||Z\r\n" +
				"OBX|3|TX|HGB|1|(Ref Range: 1 g/dL)|g/dL|14.0-24.0||||Z\r\n" +
				"OBX|4|NM|HCT||40.0|%|31.0-43.0||||Z\r\n" +
				"OBX|4|TX|HCT|1|(Ref Range: 3 %)|%|31.0-43.0||||Z\r\n" +
				"OBX|5|NM|MCV||80.0|fl|70.0-90.0||||Z\r\n" +
				"OBX|5|TX|MCV|1|(Ref Range: 7 fl)|fl|70.0-90.0||||Z\r\n" +
				"OBX|6|NM|MCH||32.0|pg|27.0-31.0|H|||Z\r\n" +
				"OBX|6|TX|MCH|1|(Ref Range: 2 pg)|pg|27.0-31.0||||Z\r\n" +
				"OBX|7|NM|MCHC||32.0|g/dL|32.0-36.0||||Z\r\n" +
				"OBX|7|TX|MCHC|1|(Ref Range: 3 g/dL)|g/dL|32.0-36.0||||Z\r\n" +
				"OBX|8|NM|RDW||17.0|%|11.5-14.5|H|||Z\r\n" +
				"OBX|8|TX|RDW|1|(Ref Range: 1 %)|%|11.5-14.5||||Z\r\n" +
				"OBX|9|NM|PLTC||2|k/uL|150-400|PL^Y|||Z\r\n" +
				"OBX|9|TX|PLTC|1|(Ref Range: 1 k/uL)|k/uL|150-400||||Z\r\n" +
				"OBX|10|NM|MPV||10.0|fL|6.2-10.0||||Z\r\n" +
				"OBX|10|TX|MPV|1|(Ref Range: 6 fL)|fL|6.2-10.0||||Z";

			PipeParser Parser = new PipeParser();

			IMessage m = Parser.Parse(Message);

			ORF_R04 orfR04 = m as ORF_R04;

			Assert.NotNull(orfR04);

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(orfR04);

			Assert.NotNull(recoveredMessage);
			Assert.False(recoveredMessage.IndexOf("NTE") > -1, "Returned Message added ORC segment.");
		}

		[Fact]
		public void TestDHPatient1111111()
		{
			PipeParser Parser = new PipeParser();

			IMessage m = Parser.Parse(GetDHPatient1111111());

			ORF_R04 orfR04 = m as ORF_R04;

			Assert.NotNull(orfR04);
			object range = orfR04.GetQUERY_RESPONSE().GetORDER().GetOBSERVATION().OBX.GetObservationValue(1);

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(orfR04);

			Assert.NotNull(recoveredMessage);
			Assert.False(recoveredMessage.IndexOf("NTE") > -1, "Returned Message added ORC segment.");
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
      <VID.1>2.3.1</VID.1>
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
		public void TestOBXDataTypes()
		{
			string message = "MSH|^~\\&|EPIC|AIDI|||20070921152053|ITFCOHIEIN|ORF^R04^ORF_R04|297|P|2.3.1|||\r\n" +
			                 "MSA|CA|1\r\n" +
			                 "QRD|20060725141358|R|||||10^RD|1130851^^^^MRN|RES|||\r\n" +
			                 "QRF|||||||||\r\n" +
			                 "OBR|1|5149916^EPC|20050118113415533318^|E8600^ECG^^^ECG|||200501181134||||||Age: 17  yrs ~Criteria: C-HP708 ~|||||1||Zztesttwocorhoi|Results||||F||^^^^^Routine|||||||||200501181134|||||||||\r\n" +
			                 "OBX|1|ST|:8601-7^ECG IMPRESSION|2|Normal sinus rhythm, rate  77     Normal P axis, PR, rate & rhythm ||||||F||1|200501181134||\r\n" +
			                 "OBX|2|ST|:8625-6^PR INTERVAL|3|141||||||F||1|200501181134||\r\n" +
			                 "OBX|3|ST|:8633-0^QRS DURATION|4|83||||||F||1|200501181134||\r\n" +
			                 "OBX|4|ST|:8634-8^QT INTERVAL|5|358||||||F||1|200501181134||\r\n" +
			                 "OBX|5|ST|:8636-3^QT INTERVAL CORRECTED|6|405||||||F||1|200501181134||\r\n" +
			                 "OBX|6|ST|:8626-4^FRONTAL AXIS P|7|-1||||||F||1|200501181134||\r\n" +
			                 "OBX|7|ST|:99003^FRONTAL AXIS INITIAL 40 MS|8|41||||||F||1|200501181134||\r\n" +
			                 "OBX|8|ST|:8632-2^FRONTAL AXIS MEAN QRS|9|66||||||F||1|200501181134||\r\n" +
			                 "OBX|9|ST|:99004^FRONTAL AXIS TERMINAL 40 MS|10|80||||||F||1|200501181134||\r\n" +
			                 "OBX|10|ST|:99005^FRONTAL AXIS ST|11|36||||||F||1|200501181134||\r\n" +
			                 "OBX|11|ST|:8638-9^FRONTAL AXIS T|12|40||||||F||1|200501181134||\r\n" +
			                 "OBX|12|ST|:99006^ECG SEVERITY T|13|- NORMAL ECG - ||||||F||1|200501181134||\r\n" +
			                 "OBX|13|DT|5315037^Start Date^Start Collection Dat^ABC||18APR06||||||F|||20060419125100|PPKMG|PPJW^SMITH, Fred \r\n" +
			                 "QAK||OK||1|1|0\r\n";

			PipeParser parser = new PipeParser();

			IMessage m = parser.Parse(message);

			ORF_R04 orfR04 = m as ORF_R04;

			Assert.NotNull(orfR04);

			XMLParser xmlParser = new DefaultXMLParser();

			string recoveredMessage = xmlParser.Encode(orfR04);
		}

		private static string GetDHPatient1111111()
		{
			return "MSH|^~\\&|Clinical Data Provider|DHHA|COHIECentral|COHIE|200609271344||ORF^R04||P|2.3.1\r\n" +
			       "MSA|AA|\r\n" +
			       "PID|1111111||1111111^^^DH^MR||DUCK^DONALD^MIDDLENAME||19600909|M\r\n" +
			       "OBR||00002^001|4514754|RAD18100388^SHOULDER MIN 2 VIEW, LT|||200609212235|||M01|||||^|124420^CHEN^YENTING^^^^EM||||||200609220854|||||1^^^^^R^^ROUTINE|||||128652&MANUEL&MISTY D&&&&RAR\r\n" +
			       "OBX|4|TX|RAD0038GDT||~ATTENDING: DOCTOR,UNASSIGNED ORDERING: CHEN, YENTING~ADMITTING: DOCTOR,UNASSIGNED PCP:   ~Result for RAD 0038 - SHOULDER MIN 2 VIEW, LT - Sep 21 2006 10:35PM  ~ACC#.4514754~REASON FOR EXAM:   6C / EXT PN~REASON FOR CHANGE:  ~FINDINGS:  Left shoulder, four views:  Bones are in normal anatomic ~alignment.  No fracture, subluxation or dislocation.  Soft tissues are ~unremarkable. ~IMPRESSION:  No evidence of acute fracture. ~~~Transcribed By: M01: 09/22/2006    ~Images and interpretation reviewed and released by:~Read by: DR. MISTY D MANUEL Reading Time: 09/21/2006  ~Reviewing Physician: DR. ELIZABETH K DEE~Approved electronically by: DR.    ~||||||F|||||OBR||00032^001|W47215|GAS^Blood Gas, Arterial Laboratory|||200609202348||||||||GAS^Blood Gas, Arterial Laboratory|888883^DOCTOR||||||||DCC\r\n" +
			       "OBX|1|NM|PH||7.10||7.37-7.45|PL^Y|||Z\r\n" +
			       "OBX|1|TX|PH||(Ref Range: 7.37-7.45 )||7.37-7.45||||F\r\n" +
			       "OBX|1|TX|PH||Critical result(s) called to and verification 'read-back' received from:  KR~||||||Z\r\n" +
			       "OBX|1|TX|PH||ISTI FORD RRNA OR1 0005 MDM||||||F\r\n" +
			       "OBX|2|NM|PCO2||55|mm Hg|34-38|H|||F\r\n" +
			       "OBX|2|TX|PCO2||(Ref Range: 34-38 mm Hg)|mm Hg|34-38||||F\r\n" +
			       "OBX|3|NM|PO2||151|mm Hg|65-75|H|||F\r\n" +
			       "OBX|3|TX|PO2||(Ref Range: 65-75 mm Hg)|mm Hg|65-75||||F\r\n" +
			       "OBX|4|NM|HCO3||16|mmol/L|20-26|L|||F\r\n" +
			       "OBX|4|TX|HCO3||(Ref Range: 20-26 mmol/L)|mmol/L|20-26||||F\r\n" +
			       "OBX|5|TX|BE||Neg 13|mmol/L|||||F\r\n" +
			       "OBX|5|TX|BE||Base Excess Reference Range: -2.5 to 1.5||||||F\r\n" +
			       "OBX|6|NM|O2SAT||98|%|91-95|H|||F\r\n" +
			       "OBX|6|TX|O2SAT||(Ref Range: 91-95 %)|%|91-95||||F\r\n" +
			       "OBX|7|NM|FIO2||100.0||||||F\r\n";
		}
	}
}