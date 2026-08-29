namespace UstediPametno.Services
{
    public interface IEmailService
    {
        Task PosaljiAsync(
            string email,
            string naslov,
            string poruka);
    }
}