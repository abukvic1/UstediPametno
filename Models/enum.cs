namespace UstediPametno.Models
{
    public enum KategorijaTroska
    {
        Stanarina,
        Rezije,
        Hrana,
        Prevoz,
        Pretplata,
        Ostalo
    }

    public enum StatusCilja
    {
        UToku,
        Ostvaren,
        Neuspjesan,
        Prilagodjen
    }

    public enum VrstaTransakcije
    {
        Prihod,
        FiksniTrosak,
        DnevnaPotrosnja,
        UplataStednje
    }
}
