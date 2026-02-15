namespace BackEnd.Models.Constants;

public static class ApplicationError
{
    public static class RequiredField
    {
        public const string NameRequired = "El nombre es obligatorio";
        public const string LastNameRequired = "El apellido es obligatorio";
        public const string EmailRequired = "El correo electrónico es obligatorio";
        public const string PasswordRequired = "La contraseña es obligatoria";
        public const string PhoneNumberRequired = "El número de teléfono es obligatorio";
    }

    public static class LengthError
    {
        public const string PasswordLength = "La contraseña debe tener al menos 8 caracteres";
    }

    public static class NotFoundError
    {
        public const string UserNotFound = "Usuario no encontrado";

    }

    public static class ValidationError
    {
        public const string ValidationFailed = "Uno o más errores de validación ocurrieron";
        public const string InvalidCredentials = "Correo electrónico o contraseña inválidos";
        public const string InvalidEmail = "Dirección de correo electrónico no válida";
        public const string InvalidPhoneNumber = "Formato de número de teléfono no válido";
    }

    public static class EmailError
    {

        public const string EmailAlreadyExists = "La dirección de correo electrónico ya existe";
    }

    public static class PhoneNumberError
    {
        public const string PhoneNumberAlreadyExists = "El número de teléfono ya existe";
    }


}