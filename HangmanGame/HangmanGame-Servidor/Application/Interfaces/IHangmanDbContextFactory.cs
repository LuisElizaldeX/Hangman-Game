namespace HangmanGame_Servidor.Application.Interfaces
{
    public interface IHangmanDbContextFactory
    {
        IHangmanDbContext Crear();
    }
}
