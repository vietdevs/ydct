using System.Collections.Generic;
using System.Threading.Tasks;

namespace newPSG.PMS.Data
{
    public interface ISqlExecuter
    {
        /// <summary>
        /// Thực thi câu truy vấn mà không cần trả về dữ liệu. Thường là các câu INSERT, UPDATE, DELETE
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int Execute(string query, params object[] parameters);

        /// <summary>
        /// Thực thi câu truy vấn và trả về danh sách các đối tượng có kiểu là K
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        List<K> Query<K>(string query, params object[] parameters);

        /// <summary>
        /// Trả về bản ghi đầu tiên của câu truy vấn. Thường là các câu lệnh query by id
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        K QueryFirst<K>(string query, params object[] parameters);

        /// <summary>
        /// Tương tự với Query<K> nhưng sử dụng Async/Await để thực thi câu truy vấn trên một Thread khác
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<List<K>> QueryAsync<K>(string query, params object[] parameters);

        /// <summary>
        /// Tương tự với QueryFirst<K> nhưng sử dụng Async/Await để thực thi câu truy vấn trên một Thread khác
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<K> QueryFirstAsync<K>(string query, params object[] parameters);

        /// <summary>
        /// Trả về giá trị tiếp theo của SEQUENCE
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="sequenceName"></param>
        /// <returns></returns>
        K GetNextSeqVal<K>(string sequenceName);
    }
}
