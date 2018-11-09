namespace StandingsGen
{
    static class TextHelper
    {
        static public FullName FullNameFinder(string s)
        {
            var inStr = s.ToLower();

            if (inStr == "маи")
                return new FullName("\"Авиаторы\"", "МАИ");
               
            if (inStr == "рэу" || inStr == "плешка")
                return new FullName("РЭУ", "им. Плеханова");

            if (inStr == "миэт" || inStr == "зеленоград")
                return new FullName("\"Электроник\"", "МИЭТ");

            if (inStr == "мгту" || inStr == "бауманка")
                return new FullName("МГТУ", "им. Баумана");

            if (inStr == "тгу" || inStr == "держава")
                return new FullName("\"Держава\"", "ТГУ");

            if (inStr == "ргуфксмит" || inStr == "гцолифк" || inStr == "ргуфк")
                return new FullName("\"Гладиаторы\"", "ГЦОЛиФК");

            if (inStr == "юургу")
                return new FullName("\"Политехник\"", "ЮУрГУ");

            if (inStr == "мсха")
                return new FullName("\"Тим. Зубры\"", "РГАУ-МСХА");

            if (inStr == "ранхигс")
                return new FullName("\"Сенатор\"", "РАНХиГС");

            if (inStr == "миит")
                return new FullName("\"Скор. машина\"", "РУТ (МИИТ)");

            if (inStr == "мисис")
                return new FullName("\"Стал. медведи\"", "МИСиС");

            if (inStr == "мифи")
                return new FullName("\"Реактор\"", "МИФИ");

            if (inStr == "лв")
                return new FullName("\"Лед. волки\"", "МАИ");

            if (inStr == "мчс")
                return new FullName("\"Огн. медведи\"", "МЧС");

            if (inStr == "фу")
                return new FullName("Финансовый", "Университет");

            if (inStr == "ннгу")
                return new FullName("ННГУ", "им. Лобачевского");

            if (inStr == "пгафксит")
                return new FullName("ПГАФКСиТ", "");

            if (inStr == "бифк")
                return new FullName("\"Алтын\"", "БИФК");

            if (inStr == "вгту")
                return new FullName("\"Дик. медведи\"", "ВГТУ");

            if (inStr == "2010")
                return new FullName("\"Авиаторы-2010\"", "МАИ");

            if (inStr == "гуу")
                return new FullName("ГУУ", "");

            return null;
        }
    }
}