using System;

namespace BusinessEntities
{
    public class CONSULTA_TICKET
    {
        ///*
        // "TIPO_PROCESO":"3", 
        //"NRO_DOCUMENTO_EMPRESA":"20100066603",
        //"USUARIO_SOL_EMPRESA":"MODDATOS",
        //"PASS_SOL_EMPRESA":"moddatos",
        //"TICKET":"1523826911770",
        //"TIPO_DOCUMENTO":"RA",
        //"NRO_DOCUMENTO":"20180415-1"
        // * /
        public Nullable<int> TIPO_PROCESO { get; set; }
        public string NRO_DOCUMENTO_EMPRESA { get; set; }
        public string USUARIO_SOL_EMPRESA { get; set; }
        public string PASS_SOL_EMPRESA { get; set; }
        public string TICKET { get; set; }
        public string TIPO_DOCUMENTO { get; set; }
        public string NRO_DOCUMENTO { get; set; }
    }
}
