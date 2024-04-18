using System.Security.Cryptography;

namespace MvcCoreProyectoSejo.Helpers
{
    public class HelperCryptography
    {
        //TENDREMOS UN METODO PARA CIFRAR EL PASSWORD, VAMOS A RECIBIR EL PASSWORD(STRING) Y EL SALT (STRING) Y
        //DEVOLVEMOS LOS BYTES DEL CIFRADO REALIZADO
        public static byte[] EncryptPassword(string password, string salt)
        {
            string contenido = password + salt;
            SHA512 sha = SHA512.Create();
            //CONVERTIMOS CONTENIDO A BYTES[] 
            byte[] salida = System.Text.Encoding.UTF8.GetBytes(contenido);
            //CREAMOS LAS ITERACIONES
            for (int i = 1; i <= 114; i++)
            {
                salida = sha.ComputeHash(salida);
            }
            sha.Clear();
            return salida;
        }
    }
}