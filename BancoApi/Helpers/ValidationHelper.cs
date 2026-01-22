using System.Text.RegularExpressions;

namespace BancoApi.Helpers;

public static class ValidationHelper
{
    // Validar UserName
    public static bool IsValidUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName)) return false;
        if (userName.Length < 8 || userName.Length > 20) return false;
        if (!Regex.IsMatch(userName, @"[A-Z]")) return false; // Al menos 1 mayúscula
        if (!Regex.IsMatch(userName, @"\d")) return false;     // Al menos 1 número
        if (Regex.IsMatch(userName, @"[^a-zA-Z0-9]")) return false; // Sin signos
        return true;
    }

    // Validar Password
    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;
        if (password.Length < 8) return false;
        if (password.Contains(" ")) return false;
        if (!Regex.IsMatch(password, @"[A-Z]")) return false;
        if (!Regex.IsMatch(password, @"[^a-zA-Z0-9]")) return false; // Al menos 1 signo
        return true;
    }

    // Validar Identificación (10 dígitos, sin 4 repetidos consecutivos)
    public static bool IsValidIdentificacion(string identificacion)
    {
        if (string.IsNullOrWhiteSpace(identificacion)) return false;
        if (identificacion.Length != 10) return false;
        if (!long.TryParse(identificacion, out _)) return false;

        // Verificar 4 dígitos repetidos consecutivos
        for (int i = 0; i <= 6; i++)
        {
            if (identificacion[i] == identificacion[i + 1] &&
                identificacion[i] == identificacion[i + 2] &&
                identificacion[i] == identificacion[i + 3])
            {
                return false;
            }
        }
        return true;
    }
}