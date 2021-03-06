﻿namespace StandingsGen
{
    static class TextHelper
    {
        static public FullName FullNameFinder(string s)
        {
            var inStr = s.ToLower();

            if (inStr == "маи")
                return new FullName("«Авиаторы»", "МАИ");

            if (inStr == "рэу" || inStr == "плешка")
                return new FullName("РЭУ", "им. Плеханова");

            if (inStr == "миэт" || inStr == "зеленоград")
                return new FullName("«Электроник»", "МИЭТ");

            if (inStr == "мгту" || inStr == "бауманка")
                return new FullName("МГТУ", "им. Баумана");

            if (inStr == "тгу" || inStr == "держава")
                return new FullName("«Держава»", "ТГУ");

            if (inStr == "ргуфксмит" || inStr == "гцолифк" || inStr == "ргуфк")
                return new FullName("«Гладиаторы»", "ГЦОЛИФК");

            if (inStr == "юургу")
                return new FullName("«Политехник»", "ЮУрГУ");

            if (inStr == "мсха")
                return new FullName("«Тим. зубры»", "РГАУ-МСХА");

            if (inStr == "ранхигс")
                return new FullName("«Сенатор»", "РАНХиГС");

            if (inStr == "миит")
                return new FullName("«Скор. машина»", "РУТ (МИИТ)");

            if (inStr == "мисис")
                return new FullName("«Стальные медведи»", "МИСиС");

            if (inStr == "мифи")
                return new FullName("«Реактор»", "МИФИ");

            if (inStr == "лв")
                return new FullName("«Ледяные волки»", "МАИ");

            if (inStr == "мчс")
                return new FullName("«Огненные медведи»", "МЧС");

            if (inStr == "фу")
                return new FullName("Финансовый", "Университет");

            if (inStr == "ннгу")
                return new FullName("ННГУ", "им. Лобачевского");

            if (inStr == "пгафксит")
                return new FullName("ПГАФКСиТ", "");

            if (inStr == "бифк")
                return new FullName("«Алтын»", "БИФК");

            if (inStr == "вгту")
                return new FullName("«Дикие медведи»", "ВГТУ");

            if (inStr == "2010")
                return new FullName("«Авиаторы-2010»", "МАИ");

            if (inStr == "гуу")
                return new FullName("ГУУ", "");

            if (inStr == "мгимо")
                return new FullName("«Дипломаты»", "МГИМО");

            if (inStr == "вшэ")
                return new FullName("«Чёрные вороны»", "ВШЭ");

            if (inStr == "мгафк")
                return new FullName("МГАФК", "");

            if (inStr == "мпгу")
                return new FullName("«Adrenaline»", "МПГУ");

            if (inStr == "мгу")
                return new FullName("МГУ", "им. Ломоносова");

            if (inStr == "ргунг")
                return new FullName("«GasOilers»", "РГУНГ");

            if (inStr == "мгюа")
                return new FullName("«Легион»", "МГЮА");

            if (inStr == "мади")
                return new FullName("«MADI Motors»", "МАДИ");

            if (inStr == "мэи")
                return new FullName("«Джокеры»", "МЭИ");

            if (inStr == "мгсу")
                return new FullName("«МГСУ»", "");

            if (inStr == "мфти")
                return new FullName("«Физтех»", "МФТИ");

            if (inStr == "тиу")
                return new FullName("ТИУ", "");

            if (inStr == "югу")
                return new FullName("ЮГУ", "");

            if (inStr == "уггу")
                return new FullName("УГГУ", "");

            if (inStr == "сфу")
                return new FullName("СФУ", "");

            if (inStr == "чгифк")
                return new FullName("«Физрук»", "ЧГИФК");

            if (inStr == "нгау")
                return new FullName("«Урожай»", "НГАУ");

            return null;
        }
    }
}