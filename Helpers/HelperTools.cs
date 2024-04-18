namespace MvcCoreProyectoSejo.Helpers
{
    public class HelperTools
    {
        public static string GenerateTokenMail()
        {
            Random random = new Random();
            string token = "";

            for (int i = 0; i <= 13; i++)
            {
                //65 - 122
                int aleat = random.Next(65, 122);
                char letra = Convert.ToChar(aleat);
                token += letra;
            }
            return token;
        }
        //VAMOS A TENER UN PAR DE METODOS QUE NO TIENEN NADA QUE VER CON CRIPTOGRAFIA
        public static string GenerateSalt()
        {
            Random random = new Random();
            string salt = "";
            for (int i = 1; i <= 50; i++)
            {
                int aleat = random.Next(1, 255);
                char letra = Convert.ToChar(aleat);
                salt += letra;
            }
            return salt;
        }
        //NECESITAMOS UN METODO PARA COMPARAR SI LOS PASSWORDS SON IGUALES, DEBEMOS COMPARAR A NIVEL DE BYTE
        public static bool CompareArrays(byte[] a, byte[] b)
        {
            bool iguales = true;
            if (a.Length != b.Length)
            {
                iguales = false;
            }
            else
            {
                for (int i = 0; i < a.Length; i++)
                {
                    //PREGUNTAMOS SI EL CNTENIDO DE CADA BYTE ES DISTINTO
                    if (a[i].Equals(b[i]) == false)
                    {
                        iguales = false;
                        break;
                    }
                }
            }
            return iguales;
        }
    }
}