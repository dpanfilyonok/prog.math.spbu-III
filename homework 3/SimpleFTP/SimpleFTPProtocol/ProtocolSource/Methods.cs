using System;

namespace ProtocolSource
{
    /// <summary>
    /// Avaliable methods of SimpleFTP protocol
    /// </summary>
    public enum Methods
    {
        /*
        Формат запроса:
        <1: Int> <path: String>
        path — путь к директории

        Формат ответа:
        <size: Int> (<name: String> <isDir: Boolean>)*,
        size — количество файлов и папок в директории
        name — название файла или папки
        isDir — флаг, принимающий значение True для директорий
        Если директории не существует, сервер посылает ответ с size = -1
         */
        List = 1,

        /*
        Формат запроса:
        <2: Int> <path: String>
        path — путь к файлу

        Формат ответа:
        <size: Int> <content: Bytes>,
        size — размер файла,
        content — его содержимое
        Если файла не существует, сервер посылает ответ с size = -1
         */
        Get = 2
    }
}
