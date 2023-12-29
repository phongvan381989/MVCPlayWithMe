using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.General
{

    /// <summary>
    /// State of Mysql result.
    /// </summary>
    public enum EMySqlResultState
    {
        /// <summary>
        /// Success.
        /// Value: 0
        /// </summary>
        OK,

        /// <summary>
        /// Error has happened.
        /// Value: 1
        /// </summary>
        ERROR,

        /// <summary>
        /// Record is exist, cant insert.
        /// Value: 2
        /// </summary>
        EXIST,

        /// <summary>
        /// Record dont exist
        /// Value: 3
        /// </summary>
        DONT_EXIST,

        /// <summary>
        /// Values have been input is invalid.
        /// value: 4
        /// </summary>
        INVALID,

        /// <summary>
        /// Has a exception.
        /// Value: 5
        /// </summary>
        EXCEPTION,

        /// <summary>
        /// Have not permission
        /// Value: 6
        /// </summary>
        AUTHEN_FAIL,

        /// <summary>
        /// Empty
        /// 7
        /// </summary>
        EMPTY,

        /// <summary>
        /// over max
        /// 8
        /// </summary>
        OVER_MAX
    }
    public class MySqlResultState
    {
        public const string authenFailMessage = "Login failed.";
        public const string LogoutMessage = "Logout success.";
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultState"/> class.
        /// Construction.
        /// </summary>
        public MySqlResultState()
        {
            this.State = EMySqlResultState.OK;
            this.Message = "Ok.";
        }

        public MySqlResultState(EMySqlResultState state, string mes)
        {
            this.State = state;
            this.Message = mes;
        }

        /// <summary>
        /// Gets or sets information of query result.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets result state.
        /// </summary>
        public EMySqlResultState State { get; set; }

        /// <summary>
        /// Chứa giá trị bất kỳ số nguyên
        /// </summary>
        public int myAnything { get; set; }

        // Chứa json của đối tượng server trả về
        public object myJson { get; set; }
    }
}