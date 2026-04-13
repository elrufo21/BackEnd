using System;
using System.Xml;
using BE = BusinessEntities;
namespace Xml
{
    public class CrearXML
    {
        public int CPE(BE.CPE comprobante, string nomArchivo, string ruta)
        {
            try
            {
                String xml;
                XmlDocument doc = new XmlDocument();
                xml = @"<?xml version='1.0' encoding='utf-8'?>
<Invoice xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:cac='urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2' xmlns:cbc='urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2' xmlns:ccts='urn:un:unece:uncefact:documentation:2' xmlns:ds='http://www.w3.org/2000/09/xmldsig#' xmlns:ext='urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2' xmlns:qdt='urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2' xmlns:udt='urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2' xmlns='urn:oasis:names:specification:ubl:schema:xsd:Invoice-2'>
	<ext:UBLExtensions>
		<ext:UBLExtension>
			<ext:ExtensionContent>
			</ext:ExtensionContent>
		</ext:UBLExtension>
	</ext:UBLExtensions>
	<cbc:UBLVersionID>2.1</cbc:UBLVersionID>
	<cbc:CustomizationID schemeAgencyName='PE:SUNAT'>2.0</cbc:CustomizationID>
	<cbc:ProfileID schemeName='Tipo de Operacion' schemeAgencyName='PE:SUNAT' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo51'>" + comprobante.TIPO_OPERACION + @"</cbc:ProfileID>
	<cbc:ID>" + comprobante.NRO_COMPROBANTE + @"</cbc:ID>
	<cbc:IssueDate>" + comprobante.FECHA_DOCUMENTO + @"</cbc:IssueDate>
    <cbc:IssueTime>" + comprobante.HORA_REGISTRO + @"</cbc:IssueTime>";
                if (comprobante.FORMA_PAGO.Equals("Contado"))
                {
                    xml = xml + @" <cbc:DueDate>" + comprobante.FECHA_VTO + @"</cbc:DueDate>";
                }
                xml = xml + @" <cbc:InvoiceTypeCode listAgencyName='PE:SUNAT' listName='Tipo de Documento' listURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo01' listID='" + comprobante.TIPO_OPERACION + "' name='Tipo de Operacion' listSchemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo51'>" + comprobante.COD_TIPO_DOCUMENTO + "</cbc:InvoiceTypeCode>";
                if ((comprobante.TOTAL_LETRAS != ""))
                    xml = xml + "<cbc:Note languageLocaleID='1000'>" + comprobante.TOTAL_LETRAS + "</cbc:Note>";
                if (comprobante.NRO_DOCUMENTO_CLIENTE.Contains("20522109178") && comprobante.TOTAL >= 700)
                {
                    xml = xml + @" <cbc:Note languageLocaleID='2006'><![CDATA[Operación sujeta a detracción]]></cbc:Note>";
                }
                xml = xml + @" <cbc:DocumentCurrencyCode listID='ISO 4217 Alpha' listName='Currency' listAgencyName='United Nations Economic Commission for Europe'>" + comprobante.COD_MONEDA + @"</cbc:DocumentCurrencyCode>
            <cbc:LineCountNumeric>" + comprobante.detalle.Count + "</cbc:LineCountNumeric>";
                if ((comprobante.NRO_OTR_COMPROBANTE != ""))
                    xml = xml + @"<cac:OrderReference>
                    <cbc:ID>" + comprobante.NRO_OTR_COMPROBANTE + @"</cbc:ID>
            </cac:OrderReference>";
                if ((comprobante.NRO_GUIA_REMISION != ""))
                    xml = xml + @"<cac:DespatchDocumentReference>
		    <cbc:ID>" + comprobante.NRO_GUIA_REMISION + @"</cbc:ID>
		    <cbc:IssueDate>" + comprobante.FECHA_GUIA_REMISION + @"</cbc:IssueDate>
		    <cbc:DocumentTypeCode listAgencyName='PE:SUNAT' listName='Tipo de Documento' listURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo01'>" + comprobante.COD_GUIA_REMISION + @"</cbc:DocumentTypeCode>
                </cac:DespatchDocumentReference>";
                xml = xml + @"
            <cac:Signature>
		<cbc:ID>" + comprobante.NRO_COMPROBANTE + @"</cbc:ID>
		<cac:SignatoryParty>
			<cac:PartyIdentification>
				<cbc:ID>" + comprobante.NRO_DOCUMENTO_EMPRESA + @"</cbc:ID>
			</cac:PartyIdentification>
			<cac:PartyName>
				<cbc:Name>" + comprobante.RAZON_SOCIAL_EMPRESA + @"</cbc:Name>
			</cac:PartyName>
		</cac:SignatoryParty>
		<cac:DigitalSignatureAttachment>
			<cac:ExternalReference>
				<cbc:URI>#" + comprobante.NRO_COMPROBANTE + @"</cbc:URI>
			</cac:ExternalReference>
		</cac:DigitalSignatureAttachment>
	</cac:Signature>
	<cac:AccountingSupplierParty>
		<cac:Party>
			<cac:PartyIdentification>
				<cbc:ID schemeID='" + comprobante.TIPO_DOCUMENTO_EMPRESA + "' schemeName='Documento de Identidad' schemeAgencyName='PE:SUNAT' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06'>" + comprobante.NRO_DOCUMENTO_EMPRESA + @"</cbc:ID>
			</cac:PartyIdentification>
			<cac:PartyName>
				<cbc:Name><![CDATA[" + comprobante.NOMBRE_COMERCIAL_EMPRESA + @"]]></cbc:Name>
			</cac:PartyName>
			<cac:PartyTaxScheme>
				<cbc:RegistrationName><![CDATA[" + comprobante.RAZON_SOCIAL_EMPRESA + @"]]></cbc:RegistrationName>
				<cbc:CompanyID schemeID='" + comprobante.TIPO_DOCUMENTO_EMPRESA + "' schemeName='SUNAT:Identificador de Documento de Identidad' schemeAgencyName='PE:SUNAT' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06'>" + comprobante.NRO_DOCUMENTO_EMPRESA + @"</cbc:CompanyID>
				<cac:TaxScheme>
					<cbc:ID schemeID='" + comprobante.TIPO_DOCUMENTO_EMPRESA + "' schemeName='SUNAT:Identificador de Documento de Identidad' schemeAgencyName='PE:SUNAT' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06'>" + comprobante.NRO_DOCUMENTO_EMPRESA + @"</cbc:ID>
				</cac:TaxScheme>
			</cac:PartyTaxScheme>
			<cac:PartyLegalEntity>
				<cbc:RegistrationName><![CDATA[" + comprobante.RAZON_SOCIAL_EMPRESA + @"]]></cbc:RegistrationName>
				<cac:RegistrationAddress>
					<cbc:ID schemeAgencyName='PE:INEI' schemeName='Ubigeos'>" + comprobante.CODIGO_UBIGEO_EMPRESA + @"</cbc:ID>
					<cbc:AddressTypeCode listAgencyName='PE:SUNAT' listName='Establecimientos anexos'>" + comprobante.CODIGO_ANEXO + @"</cbc:AddressTypeCode>
					<cbc:CityName><![CDATA[" + comprobante.DEPARTAMENTO_EMPRESA + @"]]></cbc:CityName>
					<cbc:CountrySubentity><![CDATA[" + comprobante.PROVINCIA_EMPRESA + @"]]></cbc:CountrySubentity>
					<cbc:District><![CDATA[" + comprobante.DISTRITO_EMPRESA + @"]]></cbc:District>
					<cac:AddressLine>
						<cbc:Line><![CDATA[" + comprobante.DIRECCION_EMPRESA + @"]]></cbc:Line>
					</cac:AddressLine>
					<cac:Country>
						<cbc:IdentificationCode listID='ISO 3166-1' listAgencyName='United Nations Economic Commission for Europe' listName='Country'>" + comprobante.CODIGO_PAIS_EMPRESA + @"</cbc:IdentificationCode>
					</cac:Country>
				</cac:RegistrationAddress>
			</cac:PartyLegalEntity>
			<cac:Contact>
				<cbc:Name><![CDATA[" + comprobante.CONTACTO_EMPRESA + @"]]></cbc:Name>
			</cac:Contact>
		</cac:Party>
	</cac:AccountingSupplierParty>
	<cac:AccountingCustomerParty>
		<cac:Party>
			<cac:PartyIdentification>
				<cbc:ID schemeID='" + comprobante.TIPO_DOCUMENTO_CLIENTE + "' schemeName='Documento de Identidad' schemeAgencyName='PE:SUNAT' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06'>" + comprobante.NRO_DOCUMENTO_CLIENTE + @"</cbc:ID>
			</cac:PartyIdentification>
			<cac:PartyName>
				<cbc:Name><![CDATA[" + comprobante.RAZON_SOCIAL_CLIENTE + @"]]></cbc:Name>
			</cac:PartyName>
			<cac:PartyTaxScheme>
				<cbc:RegistrationName><![CDATA[" + comprobante.RAZON_SOCIAL_CLIENTE + @"]]></cbc:RegistrationName>
				<cbc:CompanyID schemeID='" + comprobante.TIPO_DOCUMENTO_CLIENTE + "' schemeName='SUNAT:Identificador de Documento de Identidad' schemeAgencyName='PE:SUNAT' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06'>" + comprobante.NRO_DOCUMENTO_CLIENTE + @"</cbc:CompanyID>
				<cac:TaxScheme>
					<cbc:ID schemeID='" + comprobante.TIPO_DOCUMENTO_CLIENTE + "' schemeName='SUNAT:Identificador de Documento de Identidad' schemeAgencyName='PE:SUNAT' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06'>" + comprobante.NRO_DOCUMENTO_CLIENTE + @"</cbc:ID>
				</cac:TaxScheme>
			</cac:PartyTaxScheme>
			<cac:PartyLegalEntity>
				<cbc:RegistrationName><![CDATA[" + comprobante.RAZON_SOCIAL_CLIENTE + @"]]></cbc:RegistrationName>
				<cac:RegistrationAddress>
					<cbc:ID schemeName='Ubigeos' schemeAgencyName='PE:INEI'>" + comprobante.COD_UBIGEO_CLIENTE + @"</cbc:ID>
					<cbc:CityName><![CDATA[" + comprobante.DEPARTAMENTO_CLIENTE + @"]]></cbc:CityName>
					<cbc:CountrySubentity><![CDATA[" + comprobante.PROVINCIA_CLIENTE + @"]]></cbc:CountrySubentity>
					<cbc:District><![CDATA[" + comprobante.DISTRITO_CLIENTE + @"]]></cbc:District>
					<cac:AddressLine>
						<cbc:Line><![CDATA[" + comprobante.DIRECCION_CLIENTE + @"]]></cbc:Line>
					</cac:AddressLine>                                        
					<cac:Country>
						<cbc:IdentificationCode listID='ISO 3166-1' listAgencyName='United Nations Economic Commission for Europe' listName='Country'>" + comprobante.COD_PAIS_CLIENTE + @"</cbc:IdentificationCode>
					</cac:Country>
				</cac:RegistrationAddress>
			</cac:PartyLegalEntity>
		</cac:Party>
	</cac:AccountingCustomerParty>";
                if (comprobante.NRO_DOCUMENTO_CLIENTE.Contains("20522109178") && comprobante.TOTAL >= 700)
                {
                    xml = xml + @" <cac:PaymentMeans>
                   <cbc:ID>Detraccion</cbc:ID>
                   <cbc:PaymentMeansCode>001</cbc:PaymentMeansCode>
                   <cac:PayeeFinancialAccount>
                       <cbc:ID>" + comprobante.CUENTA_DETRACCION + @"</cbc:ID>
                   </cac:PayeeFinancialAccount>
                   </cac:PaymentMeans>
                   <cac:PaymentTerms>
                   <cbc:ID>Detraccion</cbc:ID>
                   <cbc:PaymentMeansID>024</cbc:PaymentMeansID>
                   <cbc:PaymentPercent>10</cbc:PaymentPercent>
                   <cbc:Amount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.MONTO_DETRACCION + @"</cbc:Amount>
                   </cac:PaymentTerms>";
                }
                if (comprobante.FORMA_PAGO.Equals("Contado"))
                {
                    xml = xml + @" <cac:PaymentTerms>
                    <cbc:ID>FormaPago</cbc:ID>
                    <cbc:PaymentMeansID>Contado</cbc:PaymentMeansID>
                    </cac:PaymentTerms>";
                }
                else
                {
                    xml = xml + @" <cac:PaymentTerms>
                <cbc:ID>FormaPago</cbc:ID>
                <cbc:PaymentMeansID>Credito</cbc:PaymentMeansID>
                <cbc:Amount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL + @"</cbc:Amount>
                </cac:PaymentTerms>";
                    xml = xml + @" <cac:PaymentTerms>
                <cbc:ID>FormaPago</cbc:ID>
                <cbc:PaymentMeansID>Cuota001</cbc:PaymentMeansID>
                <cbc:Amount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL + @"</cbc:Amount>
                <cbc:PaymentDueDate>" + comprobante.FECHA_VTO + @"</cbc:PaymentDueDate>
                </cac:PaymentTerms>";
                }
                if (comprobante.TOTAL_DESCUENTO > 0)
                {
                    //0.50000
                    xml = xml + @" <cac:AllowanceCharge>
                    <cbc:ChargeIndicator>false</cbc:ChargeIndicator>
                    <cbc:AllowanceChargeReasonCode listURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo53' listName='Cargo/descuento' listAgencyName='PE:SUNAT'>02</cbc:AllowanceChargeReasonCode>                   
                    <cbc:MultiplierFactorNumeric>" + comprobante.PORCENTAJE_DES + @"</cbc:MultiplierFactorNumeric>
                    <cbc:Amount currencyID='" + comprobante.COD_MONEDA + @"'>" + comprobante.TOTAL_DESCUENTO + @"</cbc:Amount>
                    <cbc:BaseAmount currencyID='" + comprobante.COD_MONEDA + @"'>" + comprobante.TOTAL_GRAVADAS + @"</cbc:BaseAmount>
                    </cac:AllowanceCharge>";
                }
                xml = xml + @" <cac:TaxTotal>
		<cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_IGV + @"</cbc:TaxAmount>
		<cac:TaxSubtotal>
			<cbc:TaxableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.SUB_TOTAL + @"</cbc:TaxableAmount>
			<cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_IGV + @"</cbc:TaxAmount>
			<cac:TaxCategory>
				<cbc:ID schemeID='UN/ECE 5305' schemeName='Tax Category Identifier' schemeAgencyName='United Nations Economic Commission for Europe'>S</cbc:ID>
				<cac:TaxScheme>
					<cbc:ID schemeID='UN/ECE 5153' schemeAgencyID='6'>1000</cbc:ID>
					<cbc:Name>IGV</cbc:Name>
					<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>
				</cac:TaxScheme>
			</cac:TaxCategory>
		</cac:TaxSubtotal>";
                if ((comprobante.TOTAL_ISC > 0))
                    xml = xml + @"<cac:TaxSubtotal>
            	<cbc:TaxableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_ISC + @"</cbc:TaxableAmount>
            	<cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_ISC + @"</cbc:TaxAmount>
            	<cac:TaxCategory>
            		<cbc:ID schemeID='UN/ECE 5305' schemeName='Tax Category Identifier' schemeAgencyName='United Nations Economic Commission for Europe'>S</cbc:ID>
            		<cac:TaxScheme>
            			<cbc:ID schemeID='UN/ECE 5153' schemeAgencyID='6'>2000</cbc:ID>
            			<cbc:Name>ISC</cbc:Name>
            			<cbc:TaxTypeCode>EXC</cbc:TaxTypeCode>
            		</cac:TaxScheme>
            	</cac:TaxCategory>
            </cac:TaxSubtotal>";
                if ((comprobante.TOTAL_EXPORTACION > 0))
                    xml = xml + @"<cac:TaxSubtotal>
			<cbc:TaxableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_EXPORTACION + @"</cbc:TaxableAmount>
			<cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + @"'>0.00</cbc:TaxAmount>
			<cac:TaxCategory>
				<cbc:ID schemeID='UN/ECE 5305' schemeName='Tax Category Identifier' schemeAgencyName='United Nations Economic Commission for Europe'>G</cbc:ID>
				<cac:TaxScheme>
					<cbc:ID schemeID='UN/ECE 5153' schemeAgencyID='6'>9995</cbc:ID>
					<cbc:Name>EXP</cbc:Name>
					<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>
				</cac:TaxScheme>
			</cac:TaxCategory>
		</cac:TaxSubtotal>";
                if ((comprobante.TOTAL_GRATUITAS > 0))
                    xml = xml + @"<cac:TaxSubtotal>
			<cbc:TaxableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_GRATUITAS + @"</cbc:TaxableAmount>
			<cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + @"'>0.00</cbc:TaxAmount>
			<cac:TaxCategory>
				<cbc:ID schemeID='UN/ECE 5305' schemeName='Tax Category Identifier' schemeAgencyName='United Nations Economic Commission for Europe'>Z</cbc:ID>
				<cac:TaxScheme>
					<cbc:ID schemeID='UN/ECE 5153' schemeAgencyID='6'>9996</cbc:ID>
					<cbc:Name>GRA</cbc:Name>
					<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>
				</cac:TaxScheme>
			</cac:TaxCategory>
		</cac:TaxSubtotal>";
                if ((comprobante.TOTAL_EXONERADAS > 0))
                    xml = xml + @"<cac:TaxSubtotal>
			<cbc:TaxableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_EXONERADAS + @"</cbc:TaxableAmount>
			<cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + @"'>0.00</cbc:TaxAmount>
			<cac:TaxCategory>
				<cbc:ID schemeID='UN/ECE 5305' schemeName='Tax Category Identifier' schemeAgencyName='United Nations Economic Commission for Europe'>E</cbc:ID>
				<cac:TaxScheme>
					<cbc:ID schemeID='UN/ECE 5153' schemeAgencyID='6'>9997</cbc:ID>
					<cbc:Name>EXONERADO</cbc:Name>
					<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>
				</cac:TaxScheme>
			</cac:TaxCategory>
		</cac:TaxSubtotal>";
                if ((comprobante.TOTAL_INAFECTA > 0))
                    xml = xml + @"<cac:TaxSubtotal>
			<cbc:TaxableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_INAFECTA + @"</cbc:TaxableAmount>
			<cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + @"'>0.00</cbc:TaxAmount>
			<cac:TaxCategory>
				<cbc:ID schemeID='UN/ECE 5305' schemeName='Tax Category Identifier' schemeAgencyName='United Nations Economic Commission for Europe'>O</cbc:ID>
				<cac:TaxScheme>
					<cbc:ID schemeID='UN/ECE 5153' schemeAgencyID='6'>9998</cbc:ID>
					<cbc:Name>INAFECTO</cbc:Name>
					<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>
				</cac:TaxScheme>
			</cac:TaxCategory>
		</cac:TaxSubtotal>";
                if ((comprobante.TOTAL_ICBPER > 0))
                    xml = xml + @"<cac:TaxSubtotal>
   <cbc:TaxAmount currencyID='PEN'>" + comprobante.TOTAL_ICBPER + @"</cbc:TaxAmount>
		   <cac:TaxCategory>       				  
			  <cac:TaxScheme>
				<cbc:ID schemeAgencyName='PE:SUNAT' schemeName='Codigo de tributos' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo05'>7152</cbc:ID>
				<cbc:Name>ICBPER</cbc:Name>
				<cbc:TaxTypeCode>OTH</cbc:TaxTypeCode>
			  </cac:TaxScheme>
			</cac:TaxCategory>
		</cac:TaxSubtotal>";
                if ((comprobante.TOTAL_OTR_IMP > 0))
                    xml = xml + @"<cac:TaxSubtotal>
			<cbc:TaxableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_OTR_IMP + @"</cbc:TaxableAmount>
			<cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_OTR_IMP + @"</cbc:TaxAmount>
			<cac:TaxCategory>
				<cbc:ID schemeID='UN/ECE 5305' schemeName='Tax Category Identifier' schemeAgencyName='United Nations Economic Commission for Europe'>S</cbc:ID>
				<cac:TaxScheme>
					<cbc:ID schemeID='UN/ECE 5153' schemeAgencyID='6'>9999</cbc:ID>
					<cbc:Name>OTR</cbc:Name>
					<cbc:TaxTypeCode>OTH</cbc:TaxTypeCode>
				</cac:TaxScheme>
			</cac:TaxCategory>
		</cac:TaxSubtotal>";
                xml = xml + @"</cac:TaxTotal>
	<cac:LegalMonetaryTotal>
		<cbc:LineExtensionAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.SUB_TOTAL + @"</cbc:LineExtensionAmount>
		<cbc:TaxInclusiveAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL + @"</cbc:TaxInclusiveAmount>
		<cbc:PayableRoundingAmount currencyID='" + comprobante.COD_MONEDA + @"'>0.00</cbc:PayableRoundingAmount>
		<cbc:PayableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL + @"</cbc:PayableAmount>
	</cac:LegalMonetaryTotal>";
                for (int x = 0; x <= comprobante.detalle.Count - 1; x++)
                {
                    if (comprobante.detalle[x].COD_TIPO_OPERACION == "10" || comprobante.detalle[x].COD_TIPO_OPERACION == "20" || comprobante.detalle[x].COD_TIPO_OPERACION == "30" || comprobante.detalle[x].COD_TIPO_OPERACION == "40")
                    {
                        xml = xml + @"<cac:InvoiceLine>
		            <cbc:ID>" + comprobante.detalle[x].ITEM + @"</cbc:ID>
		            <cbc:InvoicedQuantity unitCode='" + comprobante.detalle[x].UNIDAD_MEDIDA + "' unitCodeListID='UN/ECE rec 20' unitCodeListAgencyName='United Nations Economic Commission for Europe'>" + comprobante.detalle[x].CANTIDAD + @"</cbc:InvoicedQuantity>
		            <cbc:LineExtensionAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].IMPORTE + @"</cbc:LineExtensionAmount>
		            <cac:PricingReference>
			            <cac:AlternativeConditionPrice>
				            <cbc:PriceAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].PRECIO + @"</cbc:PriceAmount>
				            <cbc:PriceTypeCode listName='Tipo de Precio' listAgencyName='PE:SUNAT' listURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo16'>" + comprobante.detalle[x].PRECIO_TIPO_CODIGO + @"</cbc:PriceTypeCode>
			            </cac:AlternativeConditionPrice>
		            </cac:PricingReference>
		            <cac:TaxTotal>
			            <cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].IGV + @"</cbc:TaxAmount>";
                        xml = xml + @"<cac:TaxSubtotal>
				            <cbc:TaxableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].IMPORTE + @"</cbc:TaxableAmount>
				            <cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].IGV + @"</cbc:TaxAmount>
				            <cac:TaxCategory>
					             <cbc:ID schemeID='UN/ECE 5305' schemeAgencyID='United Nations Economic Commission for Europe'>S</cbc:ID>
					            <cbc:Percent>" + comprobante.POR_IGV + @"</cbc:Percent>
					             <cbc:TaxExemptionReasonCode listAgencyName='PE:SUNAT'
                                      listName='Afectacion del IGV'
                                      listURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07'>" + comprobante.detalle[x].COD_TIPO_OPERACION + @"</cbc:TaxExemptionReasonCode>
					            <cac:TaxScheme>
						             <cbc:ID schemeName='Codigo de tributos' schemeAgencyName='PE:SUNAT' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo05'>1000</cbc:ID>
						            <cbc:Name>IGV</cbc:Name>
						            <cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>
					            </cac:TaxScheme>
				            </cac:TaxCategory>
			            </cac:TaxSubtotal>";
                        if (comprobante.detalle[x].IMPUESTO_ICBPER > 0)
                        {
                            xml = xml + @"<cac:TaxSubtotal>
				                <cbc:TaxAmount currencyID='PEN'>" + comprobante.detalle[x].IMPUESTO_ICBPER + @"</cbc:TaxAmount>
				                <cbc:BaseUnitMeasure unitCode='NIU'>" + comprobante.detalle[x].CANTIDAD_BOLSAS + @"</cbc:BaseUnitMeasure>
				                <cac:TaxCategory>
					                <cbc:PerUnitAmount currencyID='PEN'>" + comprobante.detalle[x].SUNAT_ICBPER + @"</cbc:PerUnitAmount>
					                <cac:TaxScheme>
					                  <cbc:ID schemeAgencyName='PE:SUNAT' schemeName='Codigo de tributos' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo05'>7152</cbc:ID>
					                  <cbc:Name>ICBPER</cbc:Name>
					                  <cbc:TaxTypeCode>OTH</cbc:TaxTypeCode>	
				                    </cac:TaxScheme>
				                </cac:TaxCategory>			
			             </cac:TaxSubtotal>	";
                        }
                        if (comprobante.detalle[x].ISC > 0)
                        {
                            xml = xml + @"<cac:TaxSubtotal>
                                    <cbc:TaxableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].BI_ISC + @"</cbc:TaxableAmount>
                                    <cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].ISC + @"</cbc:TaxAmount>
                                    <cac:TaxCategory>
                                        <cbc:ID schemeID='UN/ECE 5305' schemeName='Tax Category Identifier'schemeAgencyName='United Nations Economic Commission for Europe'>S</cbc:ID>
                                        <cbc:Percent>" + comprobante.detalle[x].POR_ISC + @"</cbc:Percent>
                                        <cbc:TaxExemptionReasonCode listAgencyName='PE:SUNAT' listName='SUNAT:Codigo de Tipo de Afectaci&oacute;n del IGV' listURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07'>" + comprobante.detalle[x].COD_TIPO_OPERACION + @"</cbc:TaxExemptionReasonCode>
                                        <cbc:TierRange>" + comprobante.detalle[x].TIPO_ISC + @"</cbc:TierRange>
                                        <cac:TaxScheme>
                                            <cbc:ID schemeID='UN/ECE 5153' schemeName='Tax Scheme Identifier'schemeAgencyName='United Nations Economic Commission for Europe'>2000</cbc:ID>
                                            <cbc:Name>ISC</cbc:Name>
                                            <cbc:TaxTypeCode>EXC</cbc:TaxTypeCode>
                                        </cac:TaxScheme>
                                    </cac:TaxCategory>
                                </cac:TaxSubtotal>";
                        }
                        xml = xml + @"</cac:TaxTotal>
                        <cac:Item>
			            <cbc:Description><![CDATA[" + comprobante.detalle[x].DESCRIPCION + @"]]></cbc:Description>
			            <cac:SellersItemIdentification>
				            <cbc:ID><![CDATA[" + comprobante.detalle[x].CODIGO + @"]]></cbc:ID>
			            </cac:SellersItemIdentification>
                       <cac:CommodityClassification>
                         <cbc:ItemClassificationCode listID='UNSPSC' listAgencyName='GS1 US' listName='Item Classification'>" + comprobante.detalle[x].CODIGO_SUNAT + @"</cbc:ItemClassificationCode>
                       </cac:CommodityClassification>
                    </cac:Item>
		            <cac:Price>
			            <cbc:PriceAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].PRECIO_SIN_IMPUESTO + @"</cbc:PriceAmount>
		            </cac:Price>
	            </cac:InvoiceLine>";
                    }
                }
                xml = xml + "</Invoice>";
                doc.LoadXml(xml);
                doc.Save(ruta + nomArchivo + ".XML");
            }
            catch (Exception ex)
            {
                ex.ToString();
                return 0;
            }
            return 1;
        }
        public int CPE_NC(BE.CPE comprobante, string nomArchivo, string ruta)
        {
            try
            {
                string xml;
                XmlDocument doc = new XmlDocument();
                xml = @"<?xml version='1.0' encoding='UTF-8'?>
<CreditNote xmlns='urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2' xmlns:cac='urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2' xmlns:cbc='urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2' xmlns:ccts='urn:un:unece:uncefact:documentation:2' xmlns:ds='http://www.w3.org/2000/09/xmldsig#' xmlns:ext='urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2' xmlns:qdt='urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2' xmlns:sac='urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1' xmlns:udt='urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
    <ext:UBLExtensions>
        <ext:UBLExtension>
            <ext:ExtensionContent>
            </ext:ExtensionContent>
        </ext:UBLExtension>
    </ext:UBLExtensions>
    <cbc:UBLVersionID>2.1</cbc:UBLVersionID>
    <cbc:CustomizationID>2.0</cbc:CustomizationID>
    <cbc:ID>" + comprobante.NRO_COMPROBANTE + @"</cbc:ID>
    <cbc:IssueDate>" + comprobante.FECHA_DOCUMENTO + @"</cbc:IssueDate>
      <cbc:IssueTime>" + comprobante.HORA_REGISTRO + @"</cbc:IssueTime>";
                xml = xml + @" <cbc:DocumentCurrencyCode>" + comprobante.COD_MONEDA + @"</cbc:DocumentCurrencyCode>
    <cac:DiscrepancyResponse>
        <cbc:ReferenceID>" + comprobante.NRO_DOCUMENTO_MODIFICA + @"</cbc:ReferenceID>
        <cbc:ResponseCode>" + comprobante.COD_TIPO_MOTIVO + @"</cbc:ResponseCode>
        <cbc:Description><![CDATA[" + comprobante.DESCRIPCION_MOTIVO + @"]]></cbc:Description>
    </cac:DiscrepancyResponse>
    <cac:BillingReference>
        <cac:InvoiceDocumentReference>
            <cbc:ID>" + comprobante.NRO_DOCUMENTO_MODIFICA + @"</cbc:ID>
            <cbc:DocumentTypeCode>" + comprobante.TIPO_COMPROBANTE_MODIFICA + @"</cbc:DocumentTypeCode>
        </cac:InvoiceDocumentReference>
    </cac:BillingReference>
    <cac:Signature>
        <cbc:ID>IDSignST</cbc:ID>
        <cac:SignatoryParty>
            <cac:PartyIdentification>
                <cbc:ID>" + comprobante.NRO_DOCUMENTO_EMPRESA + @"</cbc:ID>
            </cac:PartyIdentification>
            <cac:PartyName>
                <cbc:Name><![CDATA[" + comprobante.RAZON_SOCIAL_EMPRESA + @"]]></cbc:Name>
            </cac:PartyName>
        </cac:SignatoryParty>
        <cac:DigitalSignatureAttachment>
            <cac:ExternalReference>
                <cbc:URI>#SignatureSP</cbc:URI>
            </cac:ExternalReference>
        </cac:DigitalSignatureAttachment>
    </cac:Signature>
    <cac:AccountingSupplierParty>
        <cac:Party>
            <cac:PartyIdentification>
                <cbc:ID schemeID='" + comprobante.TIPO_DOCUMENTO_EMPRESA + "' schemeName='Documento de Identidad' schemeAgencyName='PE:SUNAT' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06'>" + comprobante.NRO_DOCUMENTO_EMPRESA + @"</cbc:ID>
            </cac:PartyIdentification>
            <cac:PartyName>
                <cbc:Name><![CDATA[" + comprobante.NOMBRE_COMERCIAL_EMPRESA + @"]]></cbc:Name>
            </cac:PartyName>
            <cac:PartyLegalEntity>
<cbc:RegistrationName><![CDATA[" + comprobante.RAZON_SOCIAL_EMPRESA + @"]]></cbc:RegistrationName>
<cac:RegistrationAddress>
					<cbc:ID schemeAgencyName='PE:INEI' schemeName='Ubigeos'>" + comprobante.CODIGO_UBIGEO_EMPRESA + @"</cbc:ID>
					<cbc:AddressTypeCode listAgencyName='PE:SUNAT' listName='Establecimientos anexos'>" + comprobante.CODIGO_ANEXO + @"</cbc:AddressTypeCode>
					<cbc:CityName><![CDATA[" + comprobante.DEPARTAMENTO_EMPRESA + @"]]></cbc:CityName>
					<cbc:CountrySubentity><![CDATA[" + comprobante.PROVINCIA_EMPRESA + @"]]></cbc:CountrySubentity>
					<cbc:District><![CDATA[" + comprobante.DISTRITO_EMPRESA + @"]]></cbc:District>
					<cac:AddressLine>
						<cbc:Line><![CDATA[" + comprobante.DIRECCION_EMPRESA + @"]]></cbc:Line>
					</cac:AddressLine>
					<cac:Country>
						<cbc:IdentificationCode listID='ISO 3166-1' listAgencyName='United Nations Economic Commission for Europe' listName='Country'>" + comprobante.CODIGO_PAIS_EMPRESA + @"</cbc:IdentificationCode>
					</cac:Country>
</cac:RegistrationAddress>
            </cac:PartyLegalEntity>
        </cac:Party>
    </cac:AccountingSupplierParty>
    <cac:AccountingCustomerParty>
        <cac:Party>
            <cac:PartyIdentification>
                <cbc:ID schemeID='" + comprobante.TIPO_DOCUMENTO_CLIENTE + "' schemeName='Documento de Identidad' schemeAgencyName='PE:SUNAT' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06'>" + comprobante.NRO_DOCUMENTO_CLIENTE + @"</cbc:ID>
            </cac:PartyIdentification>
            <cac:PartyLegalEntity>
                <cbc:RegistrationName><![CDATA[" + comprobante.RAZON_SOCIAL_CLIENTE + @"]]></cbc:RegistrationName>
            </cac:PartyLegalEntity>
        </cac:Party>
    </cac:AccountingCustomerParty>
	<cac:AllowanceCharge>
		<cbc:ChargeIndicator>false</cbc:ChargeIndicator>
		<cbc:AllowanceChargeReasonCode listName='Cargo/descuento' listAgencyName='PE:SUNAT' listURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo53'>02</cbc:AllowanceChargeReasonCode>
		<cbc:MultiplierFactorNumeric>0.00</cbc:MultiplierFactorNumeric>
		<cbc:Amount currencyID='" + comprobante.COD_MONEDA + @"'>" + comprobante.TOTAL_DESCUENTO + @"</cbc:Amount>
		<cbc:BaseAmount currencyID='" + comprobante.COD_MONEDA + @"'>" + comprobante.TOTAL_GRAVADAS + @"</cbc:BaseAmount>
	</cac:AllowanceCharge>
    <cac:TaxTotal>
        <cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_IGV + @"</cbc:TaxAmount>";
                if ((comprobante.TOTAL_ICBPER > 0))
                {
                    xml = xml + @"<cac:TaxSubtotal>
   <cbc:TaxAmount currencyID='PEN'>" + comprobante.TOTAL_ICBPER + @"</cbc:TaxAmount>
		   <cac:TaxCategory>       				  
			  <cac:TaxScheme>
				<cbc:ID schemeAgencyName='PE:SUNAT' schemeName='Codigo de tributos' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo05'>7152</cbc:ID>
				<cbc:Name>ICBPER</cbc:Name>
				<cbc:TaxTypeCode>OTH</cbc:TaxTypeCode>
			  </cac:TaxScheme>
			</cac:TaxCategory>
		</cac:TaxSubtotal>";
                }
                xml = xml + @"<cac:TaxSubtotal>
<cbc:TaxableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.SUB_TOTAL + @"</cbc:TaxableAmount>
<cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_IGV + @"</cbc:TaxAmount>
            <cac:TaxCategory>
                <cac:TaxScheme>
                    <cbc:ID schemeID='UN/ECE 5153' schemeAgencyID='6'>1000</cbc:ID>
                    <cbc:Name>IGV</cbc:Name>
                    <cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>
                </cac:TaxScheme>
            </cac:TaxCategory>
        </cac:TaxSubtotal>";
                xml = xml + @"</cac:TaxTotal>
	<cac:LegalMonetaryTotal>
		<cbc:LineExtensionAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.SUB_TOTAL + @"</cbc:LineExtensionAmount>
		<cbc:TaxInclusiveAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL + @"</cbc:TaxInclusiveAmount>
		<cbc:AllowanceTotalAmount currencyID='" + comprobante.COD_MONEDA + @"'>0.00</cbc:AllowanceTotalAmount>
		<cbc:ChargeTotalAmount currencyID='" + comprobante.COD_MONEDA + @"'>0.00</cbc:ChargeTotalAmount>
		<cbc:PayableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL + @"</cbc:PayableAmount>
	</cac:LegalMonetaryTotal>";
                for (int x = 0; x <= comprobante.detalle.Count - 1; x++)
                {
                    xml = xml + @"<cac:CreditNoteLine>
        <cbc:ID>" + comprobante.detalle[x].ITEM + @"</cbc:ID>
<cbc:CreditedQuantity unitCode='" + comprobante.detalle[x].UNIDAD_MEDIDA + "'>" + comprobante.detalle[x].CANTIDAD + @"</cbc:CreditedQuantity>
<cbc:LineExtensionAmount currencyID ='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].IMPORTE + @"</cbc:LineExtensionAmount>
        <cac:PricingReference>
            <cac:AlternativeConditionPrice>
                <cbc:PriceAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].PRECIO + @"</cbc:PriceAmount>
                <cbc:PriceTypeCode>" + comprobante.detalle[x].PRECIO_TIPO_CODIGO + @"</cbc:PriceTypeCode>
            </cac:AlternativeConditionPrice>
        </cac:PricingReference>
        <cac:TaxTotal>
        <cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].IGV + @"</cbc:TaxAmount>";
                    if (comprobante.detalle[x].IMPUESTO_ICBPER > 0)
                    {
                        xml = xml + @"<cac:TaxSubtotal>
				                <cbc:TaxAmount currencyID='PEN'>" + comprobante.detalle[x].IMPUESTO_ICBPER + @"</cbc:TaxAmount>
				                <cbc:BaseUnitMeasure unitCode='NIU'>" + comprobante.detalle[x].CANTIDAD_BOLSAS + @"</cbc:BaseUnitMeasure>
				                <cac:TaxCategory>
					                 <cbc:PerUnitAmount currencyID='PEN'>" + comprobante.detalle[x].SUNAT_ICBPER + @"</cbc:PerUnitAmount>
					                <cac:TaxScheme>
					                  <cbc:ID schemeAgencyName='PE:SUNAT' schemeName='Codigo de tributos' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo05'>7152</cbc:ID>
					                  <cbc:Name>ICBPER</cbc:Name>
					                  <cbc:TaxTypeCode>OTH</cbc:TaxTypeCode>	
				                    </cac:TaxScheme>
				                </cac:TaxCategory>			
			             </cac:TaxSubtotal>	";
                    }
                    xml = xml + @"<cac:TaxSubtotal>
        <cbc:TaxableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].IMPORTE + @"</cbc:TaxableAmount>
        <cbc:TaxAmount currencyID ='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].IGV + @"</cbc:TaxAmount>
                <cac:TaxCategory>
                    <cbc:Percent>" + comprobante.POR_IGV + @"</cbc:Percent>
                    <cbc:TaxExemptionReasonCode>" + comprobante.detalle[x].COD_TIPO_OPERACION + @"</cbc:TaxExemptionReasonCode>
                    <cac:TaxScheme>
                        <cbc:ID>1000</cbc:ID>
                        <cbc:Name>IGV</cbc:Name>
                        <cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>
                    </cac:TaxScheme>
                </cac:TaxCategory>
            </cac:TaxSubtotal>
        </cac:TaxTotal>
        <cac:Item>
        <cbc:Description><![CDATA[" + comprobante.detalle[x].DESCRIPCION + @"]]></cbc:Description>
                    <cac:SellersItemIdentification>
                        <cbc:ID><![CDATA[" + comprobante.detalle[x].CODIGO + @"]]></cbc:ID>
                    </cac:SellersItemIdentification>
                       <cac:CommodityClassification>
                         <cbc:ItemClassificationCode listID='UNSPSC' listAgencyName='GS1 US' listName='Item Classification'>" + comprobante.detalle[x].CODIGO_SUNAT + @"</cbc:ItemClassificationCode>
                       </cac:CommodityClassification>
                </cac:Item>
                <cac:Price>
        <cbc:PriceAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].PRECIO_SIN_IMPUESTO + @"</cbc:PriceAmount>
                </cac:Price>
            </cac:CreditNoteLine>";
                }
                xml = xml + "</CreditNote>";
                doc.LoadXml(xml);
                doc.Save(ruta + nomArchivo + ".XML");
            }
            catch (Exception ex)
            {
                ex.ToString();
                return 0;
            }
            return 1;
        }
        public int CPE_ND(BE.CPE comprobante, string nomArchivo, string ruta)
        {
            try
            {
                string xml;
                XmlDocument doc = new XmlDocument();
                xml = @"<?xml version='1.0' encoding='UTF-8'?>
<DebitNote xmlns='urn:oasis:names:specification:ubl:schema:xsd:DebitNote-2' xmlns:cac='urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2' xmlns:cbc='urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2' xmlns:ccts='urn:un:unece:uncefact:documentation:2' xmlns:ds='http://www.w3.org/2000/09/xmldsig#' xmlns:ext='urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2' xmlns:qdt='urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2' xmlns:sac='urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1' xmlns:udt='urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
    <ext:UBLExtensions>
        <ext:UBLExtension>
            <ext:ExtensionContent>
            </ext:ExtensionContent>
        </ext:UBLExtension>
    </ext:UBLExtensions>
    <cbc:UBLVersionID>2.1</cbc:UBLVersionID>
    <cbc:CustomizationID>2.0</cbc:CustomizationID>
    <cbc:ID>" + comprobante.NRO_COMPROBANTE + @"</cbc:ID>
    <cbc:IssueDate>" + comprobante.FECHA_DOCUMENTO + @"</cbc:IssueDate>
    <cbc:IssueTime>00:00:00</cbc:IssueTime>
    <cbc:DocumentCurrencyCode>" + comprobante.COD_MONEDA + @"</cbc:DocumentCurrencyCode>
    <cac:DiscrepancyResponse>
        <cbc:ReferenceID>" + comprobante.NRO_DOCUMENTO_MODIFICA + @"</cbc:ReferenceID>
        <cbc:ResponseCode>" + comprobante.COD_TIPO_MOTIVO + @"</cbc:ResponseCode>
        <cbc:Description><![CDATA[" + comprobante.DESCRIPCION_MOTIVO + @"]]></cbc:Description>
    </cac:DiscrepancyResponse>
    <cac:BillingReference>
        <cac:InvoiceDocumentReference>
            <cbc:ID>" + comprobante.NRO_DOCUMENTO_MODIFICA + @"</cbc:ID>
            <cbc:DocumentTypeCode>" + comprobante.TIPO_COMPROBANTE_MODIFICA + @"</cbc:DocumentTypeCode>
        </cac:InvoiceDocumentReference>
    </cac:BillingReference>
    <cac:Signature>
        <cbc:ID>IDSignST</cbc:ID>
        <cac:SignatoryParty>
            <cac:PartyIdentification>
                <cbc:ID>" + comprobante.NRO_DOCUMENTO_EMPRESA + @"</cbc:ID>
            </cac:PartyIdentification>
            <cac:PartyName>
                <cbc:Name><![CDATA[" + comprobante.RAZON_SOCIAL_EMPRESA + @"]]></cbc:Name>
            </cac:PartyName>
        </cac:SignatoryParty>
        <cac:DigitalSignatureAttachment>
            <cac:ExternalReference>
                <cbc:URI>#SignatureSP</cbc:URI>
            </cac:ExternalReference>
        </cac:DigitalSignatureAttachment>
    </cac:Signature>
    <cac:AccountingSupplierParty>
        <cac:Party>
            <cac:PartyIdentification>
                <cbc:ID schemeID='" + comprobante.TIPO_DOCUMENTO_EMPRESA + "' schemeName='SUNAT:Identificador de Documento de Identidad' schemeAgencyName='PE:SUNAT' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06'>" + comprobante.NRO_DOCUMENTO_EMPRESA + @"</cbc:ID>
            </cac:PartyIdentification>
            <cac:PartyName>
                <cbc:Name><![CDATA[" + comprobante.NOMBRE_COMERCIAL_EMPRESA + @"']]></cbc:Name>
            </cac:PartyName>
            <cac:PartyLegalEntity>
                <cbc:RegistrationName><![CDATA[" + comprobante.RAZON_SOCIAL_EMPRESA + @"]]></cbc:RegistrationName>
                <cac:RegistrationAddress>
                    <cbc:AddressTypeCode>0001</cbc:AddressTypeCode>
                </cac:RegistrationAddress>
            </cac:PartyLegalEntity>
        </cac:Party>
    </cac:AccountingSupplierParty>
    <cac:AccountingCustomerParty>
        <cac:Party>
            <cac:PartyIdentification>
                <cbc:ID schemeID='" + comprobante.TIPO_DOCUMENTO_CLIENTE + "' schemeName='SUNAT:Identificador de Documento de Identidad' schemeAgencyName='PE:SUNAT' schemeURI='urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06'>" + comprobante.NRO_DOCUMENTO_CLIENTE + @"</cbc:ID>
            </cac:PartyIdentification>
            <cac:PartyLegalEntity>
<cbc:RegistrationName><![CDATA[" + comprobante.RAZON_SOCIAL_CLIENTE + @"]]></cbc:RegistrationName>
            </cac:PartyLegalEntity>
        </cac:Party>
    </cac:AccountingCustomerParty>
    <cac:TaxTotal>
        <cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_IGV + @"</cbc:TaxAmount>
        <cac:TaxSubtotal>
<cbc:TaxableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_GRAVADAS + @"</cbc:TaxableAmount>
            <cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL_IGV + @"</cbc:TaxAmount>
            <cac:TaxCategory>
                <cac:TaxScheme>
                    <cbc:ID schemeID='UN/ECE 5153' schemeAgencyID='6'>1000</cbc:ID>
                    <cbc:Name>IGV</cbc:Name>
                    <cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>
                </cac:TaxScheme>
            </cac:TaxCategory>
        </cac:TaxSubtotal>
    </cac:TaxTotal>
    <cac:RequestedMonetaryTotal>
<cbc:PayableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.TOTAL + @"</cbc:PayableAmount>
    </cac:RequestedMonetaryTotal>";
                for (int x = 0; x <= comprobante.detalle.Count - 1; x++)
                    xml = xml + @"<cac:DebitNoteLine>
        <cbc:ID>" + comprobante.detalle[x].ITEM + @"</cbc:ID>
        <cbc:DebitedQuantity unitCode='" + comprobante.detalle[x].UNIDAD_MEDIDA + "'>" + comprobante.detalle[x].CANTIDAD + @"</cbc:DebitedQuantity>
        <cbc:LineExtensionAmount currencyID ='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].IMPORTE + @"</cbc:LineExtensionAmount>
        <cac:PricingReference>
            <cac:AlternativeConditionPrice>
                <cbc:PriceAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].PRECIO + @"</cbc:PriceAmount>
                <cbc:PriceTypeCode>" + comprobante.detalle[x].PRECIO_TIPO_CODIGO + @"</cbc:PriceTypeCode>
            </cac:AlternativeConditionPrice>
        </cac:PricingReference>
        <cac:TaxTotal>
        <cbc:TaxAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].IGV + @"</cbc:TaxAmount>
            <cac:TaxSubtotal>
                <cbc:TaxableAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].IMPORTE + @"</cbc:TaxableAmount>
                <cbc:TaxAmount currencyID ='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].IGV + @"</cbc:TaxAmount>
                <cac:TaxCategory>
                    <cbc:Percent>" + comprobante.POR_IGV + @"</cbc:Percent>
                    <cbc:TaxExemptionReasonCode>" + comprobante.detalle[x].COD_TIPO_OPERACION + @"</cbc:TaxExemptionReasonCode>
                    <cac:TaxScheme>
                        <cbc:ID>1000</cbc:ID>
                        <cbc:Name>IGV</cbc:Name>
                        <cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>
                    </cac:TaxScheme>
                </cac:TaxCategory>
            </cac:TaxSubtotal>
        </cac:TaxTotal>		
        <cac:Item>
            <cbc:Description><![CDATA[" + comprobante.detalle[x].DESCRIPCION + @"]]></cbc:Description>
            <cac:SellersItemIdentification>
                    <cbc:ID><![CDATA[" + comprobante.detalle[x].CODIGO + @"]]></cbc:ID>
            </cac:SellersItemIdentification>
        </cac:Item>
        <cac:Price>
        <cbc:PriceAmount currencyID='" + comprobante.COD_MONEDA + "'>" + comprobante.detalle[x].PRECIO + @"</cbc:PriceAmount>
        </cac:Price>
        </cac:DebitNoteLine>";

                xml = xml + "</DebitNote>";

                doc.LoadXml(xml);
                doc.Save(ruta + nomArchivo + ".XML");
            }
            catch (Exception ex)
            {
                ex.ToString();
                return 0;
            }
            return 1;
        }
        public void ResumenBoleta(BE.CPE_RESUMEN_BOLETA comprobante, string nomArchivo, string ruta)
        {
            try
            {
                string xml;
                XmlDocument doc = new XmlDocument();
                xml = @"<?xml version='1.0' encoding='ISO-8859-1' standalone='no'?>
                    <SummaryDocuments xmlns='urn:sunat:names:specification:ubl:peru:schema:xsd:SummaryDocuments-1' 
                    xmlns:cac='urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2' 
                    xmlns:cbc='urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2' 
                    xmlns:ds='http://www.w3.org/2000/09/xmldsig#' 
                    xmlns:ext='urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2' 
                    xmlns:qdt='urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2' 
                    xmlns:sac='urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1' 
                    xmlns:udt='urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2'>
                    <ext:UBLExtensions>
                    <ext:UBLExtension>
                    <ext:ExtensionContent>
                    </ext:ExtensionContent>
                    </ext:UBLExtension>
                    </ext:UBLExtensions>
                    <cbc:UBLVersionID>2.0</cbc:UBLVersionID>
                    <cbc:CustomizationID>1.1</cbc:CustomizationID>
                    <cbc:ID>" + comprobante.CODIGO + "-" + comprobante.SERIE + "-" + comprobante.SECUENCIA + @"</cbc:ID>
                    <cbc:ReferenceDate>" + comprobante.FECHA_REFERENCIA + @"</cbc:ReferenceDate>
                    <cbc:IssueDate>" + comprobante.FECHA_DOCUMENTO + @"</cbc:IssueDate>
                    <cac:Signature>
                    <cbc:ID>" + comprobante.CODIGO + "-" + comprobante.SERIE + "-" + comprobante.SECUENCIA + @"</cbc:ID>
                    <cac:SignatoryParty>
                    <cac:PartyIdentification>
                    <cbc:ID>" + comprobante.NRO_DOCUMENTO_EMPRESA + @"</cbc:ID>
                    </cac:PartyIdentification>
                    <cac:PartyName>
                    <cbc:Name><![CDATA[" + comprobante.RAZON_SOCIAL + @"]]></cbc:Name>
                    </cac:PartyName>
                    </cac:SignatoryParty>
                    <cac:DigitalSignatureAttachment>
                    <cac:ExternalReference>
                    <cbc:URI>" + comprobante.CODIGO + "-" + comprobante.SERIE + "-" + comprobante.SECUENCIA + @"</cbc:URI>
                    </cac:ExternalReference>
                    </cac:DigitalSignatureAttachment>
                    </cac:Signature>
                    <cac:AccountingSupplierParty>
                    <cbc:CustomerAssignedAccountID>" + comprobante.NRO_DOCUMENTO_EMPRESA + @"</cbc:CustomerAssignedAccountID>
                    <cbc:AdditionalAccountID>6</cbc:AdditionalAccountID>
                    <cac:Party>
                    <cac:PartyLegalEntity>
                    <cbc:RegistrationName><![CDATA[" + comprobante.RAZON_SOCIAL + @"]]></cbc:RegistrationName>
                    </cac:PartyLegalEntity>
                    </cac:Party>
                    </cac:AccountingSupplierParty>";
                for (int x = 0; x <= comprobante.detalle.Count - 1; x++)
                {
                    xml = xml + @"<sac:SummaryDocumentsLine>
                    <cbc:LineID>" + comprobante.detalle[x].ITEM + @"</cbc:LineID>
                    <cbc:DocumentTypeCode>" + comprobante.detalle[x].TIPO_COMPROBANTE + @"</cbc:DocumentTypeCode>
                    <cbc:ID>" + comprobante.detalle[x].NRO_COMPROBANTE + @"</cbc:ID>
                    <cac:AccountingCustomerParty>
                    <cbc:CustomerAssignedAccountID>" + comprobante.detalle[x].NRO_DOCUMENTO + @"</cbc:CustomerAssignedAccountID>
                    <cbc:AdditionalAccountID>" + comprobante.detalle[x].TIPO_DOCUMENTO + @"</cbc:AdditionalAccountID>
                    </cac:AccountingCustomerParty>";
                    if ((comprobante.detalle[x].TIPO_COMPROBANTE == "07" || comprobante.detalle[x].TIPO_COMPROBANTE == "08"))
                        xml = xml + @"<cac:BillingReference>
			                        <cac:InvoiceDocumentReference>
				                        <cbc:ID>" + comprobante.detalle[x].NRO_COMPROBANTE_REF + @"</cbc:ID>
				                        <cbc:DocumentTypeCode>" + comprobante.detalle[x].TIPO_COMPROBANTE_REF + @"</cbc:DocumentTypeCode>
			                        </cac:InvoiceDocumentReference>
		                        </cac:BillingReference>";
                    xml = xml + @"<cac:Status>
                    <cbc:ConditionCode>" + comprobante.detalle[x].STATU + @"</cbc:ConditionCode>
                    </cac:Status>
                    <sac:TotalAmount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].TOTAL + @"</sac:TotalAmount>
                    <sac:BillingPayment>
                    <cbc:PaidAmount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].GRAVADA + @"</cbc:PaidAmount>
                    <cbc:InstructionID>01</cbc:InstructionID>
                    </sac:BillingPayment>";
                    if ((comprobante.detalle[x].EXONERADO > 0))
                        xml = xml + @"<sac:BillingPayment>
                    <cbc:PaidAmount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].EXONERADO + @"</cbc:PaidAmount>
                    <cbc:InstructionID>02</cbc:InstructionID>
                    </sac:BillingPayment>";

                    if ((comprobante.detalle[x].INAFECTO > 0))
                        xml = xml + @"<sac:BillingPayment>
                    <cbc:PaidAmount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].INAFECTO + @"</cbc:PaidAmount>
                    <cbc:InstructionID>03</cbc:InstructionID>
                    </sac:BillingPayment>";

                    if ((comprobante.detalle[x].EXPORTACION > 0))
                        xml = xml + @"<sac:BillingPayment>
                    <cbc:PaidAmount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].EXPORTACION + @"</cbc:PaidAmount>
                    <cbc:InstructionID>04</cbc:InstructionID>
                    </sac:BillingPayment>";

                    if ((comprobante.detalle[x].GRATUITAS > 0))
                        xml = xml + @"<sac:BillingPayment>
                    <cbc:PaidAmount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].GRATUITAS + @"</cbc:PaidAmount>
                    <cbc:InstructionID>05</cbc:InstructionID>
                    </sac:BillingPayment>";
                    if ((comprobante.detalle[x].MONTO_CARGO_X_ASIG > 0))
                    {
                        xml = xml + "<cac:AllowanceCharge>";
                        if ((comprobante.detalle[x].CARGO_X_ASIGNACION == 1))
                            xml = xml + "<cbc:ChargeIndicator>true</cbc:ChargeIndicator>";
                        else
                            xml = xml + "<cbc:ChargeIndicator>false</cbc:ChargeIndicator>";
                        xml = xml + "<cbc:Amount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].MONTO_CARGO_X_ASIG + @"</cbc:Amount>
                    </cac:AllowanceCharge>";
                    }
                    if ((comprobante.detalle[x].ISC > 0))
                        xml = xml + @"<cac:TaxTotal>
			        <cbc:TaxAmount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].ISC + @"</cbc:TaxAmount>
			        <cac:TaxSubtotal>
                        <cbc:TaxAmount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].ISC + @"</cbc:TaxAmount>
				        <cac:TaxCategory>
                            <cac:TaxScheme>
                                <cbc:ID>2000</cbc:ID>
                                <cbc:Name>ISC</cbc:Name>
                                <cbc:TaxTypeCode>EXC</cbc:TaxTypeCode>
                            </cac:TaxScheme>
				        </cac:TaxCategory>
                    </cac:TaxSubtotal>
		        </cac:TaxTotal>";
                    if ((comprobante.detalle[x].ICBPER > 0))
                    {
                        xml = xml + @"<cac:TaxTotal>
                    <cbc:TaxAmount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].ICBPER + @"</cbc:TaxAmount>
                    <cac:TaxSubtotal>
                    <cbc:TaxAmount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].ICBPER + @"</cbc:TaxAmount>
                    <cac:TaxCategory>
                    <cac:TaxScheme>
                    <cbc:ID>7152</cbc:ID>
                    <cbc:Name>ICBPER</cbc:Name>
                    <cbc:TaxTypeCode>OTH</cbc:TaxTypeCode>
                    </cac:TaxScheme>
                    </cac:TaxCategory>
                    </cac:TaxSubtotal>
                    </cac:TaxTotal>";
                    }
                    xml = xml + @"<cac:TaxTotal>
                    <cbc:TaxAmount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].IGV + @"</cbc:TaxAmount>
                    <cac:TaxSubtotal>
                    <cbc:TaxAmount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].IGV + @"</cbc:TaxAmount>
                    <cac:TaxCategory>
                    <cac:TaxScheme>
                    <cbc:ID>1000</cbc:ID>
                    <cbc:Name>IGV</cbc:Name>
                    <cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>
                    </cac:TaxScheme>
                    </cac:TaxCategory>
                    </cac:TaxSubtotal>
                    </cac:TaxTotal>";

                    if ((comprobante.detalle[x].OTROS > 0))
                        xml = xml + @"<cac:TaxTotal>
			            <cbc:TaxAmount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].OTROS + @"</cbc:TaxAmount>
			            <cac:TaxSubtotal>
                            <cbc:TaxAmount currencyID='" + comprobante.detalle[x].COD_MONEDA + "'>" + comprobante.detalle[x].OTROS + @"</cbc:TaxAmount>
				            <cac:TaxCategory>
                                <cac:TaxScheme>
                                    <cbc:ID>9999</cbc:ID>
                                    <cbc:Name>OTROS</cbc:Name>
                                    <cbc:TaxTypeCode>OTH</cbc:TaxTypeCode>
                                </cac:TaxScheme>
				            </cac:TaxCategory>
                        </cac:TaxSubtotal>
		            </cac:TaxTotal>";

                    xml = xml + "</sac:SummaryDocumentsLine>";
                }
                xml = xml + "</SummaryDocuments>";
                doc.LoadXml(xml);
                doc.Save(ruta + nomArchivo + ".XML");
            }
            catch (Exception ex)
            {
                ex.ToString();
                //return 0;
            }
        }
        public void ResumenBaja(BE.CPE_BAJA comprobante, string nomArchivo, string ruta)
        {
            try
            {
                string xml;
                XmlDocument doc = new XmlDocument();
                xml = @"<?xml version='1.0' encoding='ISO-8859-1' standalone='no'?>
                    <VoidedDocuments xmlns='urn:sunat:names:specification:ubl:peru:schema:xsd:VoidedDocuments-1' 
                    xmlns:cac='urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2' 
                    xmlns:cbc='urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2' 
                    xmlns:ds='http://www.w3.org/2000/09/xmldsig#' xmlns:ext='urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2' 
                    xmlns:sac='urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1' 
                    xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
	                    <ext:UBLExtensions>
		                    <ext:UBLExtension>
			                    <ext:ExtensionContent>
			                    </ext:ExtensionContent>
		                    </ext:UBLExtension>
	                    </ext:UBLExtensions>
	                    <cbc:UBLVersionID>2.0</cbc:UBLVersionID>
	                    <cbc:CustomizationID>1.0</cbc:CustomizationID>
	                    <cbc:ID>" + comprobante.CODIGO + "-" + comprobante.SERIE + "-" + comprobante.SECUENCIA + @"</cbc:ID>
	                    <cbc:ReferenceDate>" + comprobante.FECHA_REFERENCIA + @"</cbc:ReferenceDate>
	                    <cbc:IssueDate>" + comprobante.FECHA_BAJA + @"</cbc:IssueDate>
	                    <cac:Signature>
		                    <cbc:ID>" + comprobante.CODIGO + "-" + comprobante.SERIE + "-" + comprobante.SECUENCIA + @"</cbc:ID>
		                    <cac:SignatoryParty>
			                    <cac:PartyIdentification>
				                    <cbc:ID>" + comprobante.NRO_DOCUMENTO_EMPRESA + @"</cbc:ID>
			                    </cac:PartyIdentification>
			                    <cac:PartyName>
				                    <cbc:Name>" + comprobante.RAZON_SOCIAL + @"</cbc:Name>
			                    </cac:PartyName>
		                    </cac:SignatoryParty>
		                    <cac:DigitalSignatureAttachment>
			                    <cac:ExternalReference>
				                    <cbc:URI>" + comprobante.CODIGO + "-" + comprobante.SERIE + "-" + comprobante.SECUENCIA + @"</cbc:URI>
			                    </cac:ExternalReference>
		                    </cac:DigitalSignatureAttachment>
	                    </cac:Signature>
	                    <cac:AccountingSupplierParty>
		                    <cbc:CustomerAssignedAccountID>" + comprobante.NRO_DOCUMENTO_EMPRESA + @"</cbc:CustomerAssignedAccountID>
		                    <cbc:AdditionalAccountID>" + comprobante.TIPO_DOCUMENTO + @"</cbc:AdditionalAccountID>
		                    <cac:Party>
			                    <cac:PartyLegalEntity>
				                    <cbc:RegistrationName>" + comprobante.RAZON_SOCIAL + @"</cbc:RegistrationName>
			                    </cac:PartyLegalEntity>
		                    </cac:Party>
	                    </cac:AccountingSupplierParty>";
                for (int x = 0; x <= comprobante.detalle.Count - 1; x++)
                    xml = xml + @"<sac:VoidedDocumentsLine>
		                     <cbc:LineID>" + comprobante.detalle[x].ITEM + @"</cbc:LineID>
		                     <cbc:DocumentTypeCode>" + comprobante.detalle[x].TIPO_COMPROBANTE + @"</cbc:DocumentTypeCode>
		                     <sac:DocumentSerialID>" + comprobante.detalle[x].SERIE + @"</sac:DocumentSerialID>
		                     <sac:DocumentNumberID>" + comprobante.detalle[x].NUMERO + @"</sac:DocumentNumberID>
		                     <sac:VoidReasonDescription>" + comprobante.detalle[x].DESCRIPCION + @"</sac:VoidReasonDescription>
	                         </sac:VoidedDocumentsLine>";
                xml = xml + "</VoidedDocuments>";
                doc.LoadXml(xml);
                doc.Save(ruta + nomArchivo + ".XML");
            }
            catch (Exception ex)
            {
                ex.ToString();
                //return 0;
            }
        }
    }
}