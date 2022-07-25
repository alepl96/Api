using System;
namespace ApiTest
{
    public class funciones
    {
        public static bool f_validar_correo(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


    }
}
