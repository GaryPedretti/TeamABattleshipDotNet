namespace Battleship.GameController.Contracts
{
    /// <summary>
    ///     The position.
    /// </summary>
    public class Position
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="row">
        /// The row.
        /// </param>
        public Position(Letters column, int row)
        {
            Column = column;
            Row = row;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Position" /> class.
        /// </summary>
        public Position()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the column.
        /// </summary>
        public Letters Column { get; set; }

        /// <summary>
        ///     Gets or sets the row.
        /// </summary>
        public int Row { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var position = obj as Position;
            if (position == null)
            {
                return false;
            }

            return position.Column == Column && position.Row == Row;
        }

        #endregion
    }
}