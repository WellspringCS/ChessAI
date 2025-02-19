﻿using System.Text;
using ChessAI.Game;

namespace ChessAI.Models;

public class ChessBoard
{
    // 2D array representing the chessboard where each element holds a ChessPiece object or null (if the square is empty).
    public ChessPiece[,] Pieces { get; } = new ChessPiece[8, 8];

    // Tracks the overall game state (active player, move counters, etc.)
    public GameState State { get; } = new();

    // Tracks castling rights and whether specific pieces have moved
    public CastlingRights Castling { get; } = new();

    // Tracks en passant information
    public EnPassant EnPassantTarget { get; } = new();


    // 
    //public ChessPiece[,] Pieces { get; } = new ChessPiece[8, 8];

    // Tracks which color's turn it currently is (either White or Black).
    //public ChessPiece.PieceColor ActiveColor { get; set; } = ChessPiece.PieceColor.White;

    // Indicates whether White can still castle kingside (with the h1 rook).
    public bool WhiteKingsideCastle { get; } = true;

    // Indicates whether White can still castle queenside (with the a1 rook).
    public bool WhiteQueensideCastle { get; } = true;

    // Indicates whether Black can still castle kingside (with the h8 rook).
    public bool BlackKingsideCastle { get; } = true;

    // Indicates whether Black can still castle queenside (with the a8 rook).
    public bool BlackQueensideCastle { get; } = true;

    //// Tracks whether White's king has moved (if true, White can't castle anymore).
    //public bool WhiteKingMoved { get; private set; }

    //// Tracks whether Black's king has moved (if true, Black can't castle anymore).
    //public bool BlackKingMoved { get; private set; }

    //// Tracks whether White's kingside rook (h1) has moved (if true, White can't castle kingside).
    //public bool WhiteKingsideRookMoved { get; private set; }

    //// Tracks whether White's queenside rook (a1) has moved (if true, White can't castle queenside).
    //public bool WhiteQueensideRookMoved { get; private set; }

    //// Tracks whether Black's kingside rook (h8) has moved (if true, Black can't castle kingside).
    //public bool BlackKingsideRookMoved { get; private set; }

    //// Tracks whether Black's queenside rook (a8) has moved (if true, Black can't castle queenside).
    //public bool BlackQueensideRookMoved { get; private set; }

    // Stores the square where en passant capture is possible, or "-" if no en passant is available.
    //public string EnPassantTarget { get; private set; } = "-";

    // Tracks the number of halfmoves since the last capture or pawn move (used for the fifty-move rule).
    public int HalfmoveClock { get; }

    // Tracks the total number of full moves in the game (incremented after Black’s move).
    public int FullmoveNumber { get; } = 1;

    #region Initialization

    /// <summary>
    ///     Initializes the board by placing pieces in their starting positions for both players.
    /// </summary>
    public void InitializeBoard()
    {
        var initialWhitePieces = new ChessPiece[]
        {
            new(ChessPiece.PieceType.Rook, ChessPiece.PieceColor.White, 0, 0),
            new(ChessPiece.PieceType.Knight, ChessPiece.PieceColor.White, 0, 1),
            new(ChessPiece.PieceType.Bishop, ChessPiece.PieceColor.White, 0, 2),
            new(ChessPiece.PieceType.Queen, ChessPiece.PieceColor.White, 0, 3),
            new(ChessPiece.PieceType.King, ChessPiece.PieceColor.White, 0, 4),
            new(ChessPiece.PieceType.Bishop, ChessPiece.PieceColor.White, 0, 5),
            new(ChessPiece.PieceType.Knight, ChessPiece.PieceColor.White, 0, 6),
            new(ChessPiece.PieceType.Rook, ChessPiece.PieceColor.White, 0, 7)
        };

        var initialBlackPieces = new ChessPiece[]
        {
            new(ChessPiece.PieceType.Rook, ChessPiece.PieceColor.Black, 7, 0),
            new(ChessPiece.PieceType.Knight, ChessPiece.PieceColor.Black, 7, 1),
            new(ChessPiece.PieceType.Bishop, ChessPiece.PieceColor.Black, 7, 2),
            new(ChessPiece.PieceType.Queen, ChessPiece.PieceColor.Black, 7, 3),
            new(ChessPiece.PieceType.King, ChessPiece.PieceColor.Black, 7, 4),
            new(ChessPiece.PieceType.Bishop, ChessPiece.PieceColor.Black, 7, 5),
            new(ChessPiece.PieceType.Knight, ChessPiece.PieceColor.Black, 7, 6),
            new(ChessPiece.PieceType.Rook, ChessPiece.PieceColor.Black, 7, 7)
        };

        for (var i = 0; i < 8; i++)
        {
            Pieces[0, i] = initialWhitePieces[i];
            Pieces[1, i] = new ChessPiece(ChessPiece.PieceType.Pawn, ChessPiece.PieceColor.White, 1, i);
            Pieces[6, i] = new ChessPiece(ChessPiece.PieceType.Pawn, ChessPiece.PieceColor.Black, 6, i);
            Pieces[7, i] = initialBlackPieces[i];
        }
    }

    #endregion

    #region Castling

    ///// <summary>
    /////     Checks whether castling is allowed for the given color and direction (kingside or queenside).
    ///// </summary>
    //public bool CanCastle(ChessPiece.PieceColor color, bool kingside)
    //{
    //    var row = color == ChessPiece.PieceColor.White ? 0 : 7;

    //    // Check if the king or rook has moved
    //    if (color == ChessPiece.PieceColor.White)
    //    {
    //        if (WhiteKingMoved || (kingside && WhiteKingsideRookMoved) || (!kingside && WhiteQueensideRookMoved))
    //            return false;
    //    }
    //    else
    //    {
    //        if (BlackKingMoved || (kingside && BlackKingsideRookMoved) || (!kingside && BlackQueensideRookMoved))
    //            return false;
    //    }

    //    // Determine the start and end columns for the path check
    //    var startCol = kingside ? 5 : 1;
    //    var endCol = kingside ? 6 : 3;

    //    // Check if the path is clear and not under attack
    //    for (var col = startCol; col <= endCol; col++)
    //        if (Pieces[row, col] != null || IsSquareUnderAttack(row, col, color))
    //            return false;

    //    return true;
    //}

    ///// <summary>
    /////     Applies the castling move for the given color and direction.
    ///// </summary>
    //public void ApplyCastlingMove(ChessPiece.PieceColor color, bool kingside)
    //{
    //    var row = color == ChessPiece.PieceColor.White ? 0 : 7;
    //    var kingStartCol = 4;
    //    var rookStartCol = kingside ? 7 : 0;
    //    var kingEndCol = kingside ? 6 : 2;
    //    var rookEndCol = kingside ? 5 : 3;

    //    MovePiece(Pieces[row, kingStartCol], row, kingEndCol);
    //    MovePiece(Pieces[row, rookStartCol], row, rookEndCol);

    //    if (color == ChessPiece.PieceColor.White)
    //    {
    //        WhiteKingMoved = true;
    //        if (kingside) WhiteKingsideRookMoved = true;
    //        else WhiteQueensideRookMoved = true;
    //    }
    //    else
    //    {
    //        BlackKingMoved = true;
    //        if (kingside) BlackKingsideRookMoved = true;
    //        else BlackQueensideRookMoved = true;
    //    }
    //}

    #endregion

    #region Piece Movement

    /// <summary>
    ///     Moves a piece to a new position, updating the board's state.
    /// </summary>
    public void MovePiece(ChessPiece piece, int newRow, int newCol)
    {
        Pieces[piece.Row, piece.Col] = null;
        Pieces[newRow, newCol] = piece;
        piece.Row = newRow;
        piece.Col = newCol;
    }

    /// <summary>
    ///     Applies a move to the board. Handles special cases like castling and updates the game state.
    /// </summary>
    public void ApplyMove(ChessMove move)
    {
        var piece = Pieces[move.FromRow, move.FromCol];
        if (piece != null)
        {
            // Detect castling attempt
            if (piece.Type == ChessPiece.PieceType.King && Math.Abs(move.ToCol - move.FromCol) == 2)
            {
                var kingside = move.ToCol > move.FromCol;
                if (Castling.CanCastle(piece.Color, kingside))
                {
                    Castling.ApplyCastlingMove(piece.Color, kingside, this);
                    State.ToggleActiveColor();
                    return;
                }
            }

            MovePiece(piece, move.ToRow, move.ToCol);
            State.UpdateHalfmoveClock(piece, move, Pieces);
            UpdateGameState(piece, move);

        }
    }

    /// <summary>
    ///     Updates the game state after a move. This includes checking for checkmate or stalemate,
    ///     updating castling rights, and handling en passant.
    /// </summary>
    private void UpdateGameState(ChessPiece piece, ChessMove move)
    {
        // Update game state
        State.ToggleActiveColor();

        // Check for checkmate or stalemate
        if (IsCheckmate(State.ActiveColor))
        {
            Console.WriteLine($"{State.ActiveColor} is in checkmate! Game over.");
            return;
        }

        if (IsStalemate(State.ActiveColor))
        {
            Console.WriteLine("Stalemate! The game is a draw.");
            return;
        }

        // Update castling rights if a rook or king moves
        Castling.UpdateCastlingRights(piece, move);

        // Update en passant target
        EnPassantTarget.UpdateTarget(piece, move);

        // Update halfmove clock
        State.UpdateHalfmoveClock(piece, move, Pieces);

        // Update fullmove number
        State.UpdateFullmoveNumber();

        //// Update castling rights if a rook or king moves
        //Castling.UpdateCastlingRights(piece, move);

        //// Update en passant target
        //EnPassantTarget.UpdateTarget(piece, move);

        //// Update halfmove clock
        //if (piece.Type == ChessPiece.PieceType.Pawn || Pieces[move.ToRow, move.ToCol] != null)
        //    HalfmoveClock = 0;
        //else
        //    HalfmoveClock++;

        //// Update fullmove number
        //if (ActiveColor == ChessPiece.PieceColor.White)
        //    FullmoveNumber++;
    }

    ///// <summary>
    /////     Updates castling rights when a rook or king moves.
    ///// </summary>
    //private void UpdateCastlingRights(ChessPiece piece, ChessMove move)
    //{
    //    if (piece.Type == ChessPiece.PieceType.King)
    //    {
    //        if (piece.Color == ChessPiece.PieceColor.White)
    //        {
    //            WhiteKingsideCastle = false;
    //            WhiteQueensideCastle = false;
    //        }
    //        else
    //        {
    //            BlackKingsideCastle = false;
    //            BlackQueensideCastle = false;
    //        }
    //    }
    //    else if (piece.Type == ChessPiece.PieceType.Rook)
    //    {
    //        if (piece.Color == ChessPiece.PieceColor.White)
    //        {
    //            if (move.FromCol == 0) WhiteQueensideCastle = false;
    //            if (move.FromCol == 7) WhiteKingsideCastle = false;
    //        }
    //        else
    //        {
    //            if (move.FromCol == 0) BlackQueensideCastle = false;
    //            if (move.FromCol == 7) BlackKingsideCastle = false;
    //        }
    //    }
    //}

    ///// <summary>
    /////     Updates the en passant target when a pawn moves two squares forward.
    ///// </summary>
    //private void UpdateEnPassantTarget(ChessPiece piece, ChessMove move)
    //{
    //    if (piece.Type == ChessPiece.PieceType.Pawn && Math.Abs(move.ToRow - move.FromRow) == 2)
    //    {
    //        var enPassantRow = (move.FromRow + move.ToRow) / 2;
    //        var enPassantCol = (char)('a' + move.FromCol);
    //        EnPassantTarget = $"{enPassantCol}{enPassantRow + 1}";
    //    }
    //    else
    //    {
    //        EnPassantTarget = "-";
    //    }
    //}

    #endregion

    #region Check and Checkmate

    /// <summary>
    ///     Determines if the given color's king is in check.
    /// </summary>
    public bool IsInCheck(ChessPiece.PieceColor color)
    {
        var king = GetKing(color);
        return king != null && IsSquareUnderAttack(king.Row, king.Col, color);
    }

    /// <summary>
    ///     Determines if the given color is in checkmate.
    /// </summary>
    public bool IsCheckmate(ChessPiece.PieceColor color)
    {
        if (!IsInCheck(color)) return false;

        foreach (var piece in GetPieces(color))
        {
            var moves = GetValidMoves(piece);
            if (moves.Any()) return false;
        }

        return true;
    }

    /// <summary>
    ///     Determines if the game is in stalemate for the given color.
    /// </summary>
    public bool IsStalemate(ChessPiece.PieceColor color)
    {
        if (IsInCheck(color)) return false;

        foreach (var piece in GetPieces(color))
        {
            var moves = GetValidMoves(piece);
            if (moves.Any()) return false;
        }

        return true;
    }

    #endregion

    #region Valid Moves and Attacks

    /// <summary>
    ///     Returns a list of valid moves for the given piece, ensuring no moves leave the king in check.
    /// </summary>
    public List<(int, int)> GetValidMoves(ChessPiece piece)
    {
        var moves = GetValidMovesWithoutCheckTest(piece);

        // Filter out moves that would leave the king in check
        return moves.Where(move => !WouldLeaveKingInCheck(piece, move)).ToList();
    }

    /// <summary>
    ///     Returns a list of all possible moves for a piece without checking if they would leave the king in check.
    /// </summary>
    private List<(int, int)> GetValidMovesWithoutCheckTest(ChessPiece piece)
    {
        var moves = new List<(int, int)>();

        switch (piece.Type)
        {
            case ChessPiece.PieceType.Pawn:
                moves.AddRange(GetPawnMoves(piece));
                break;
            case ChessPiece.PieceType.Rook:
                moves.AddRange(GetLinearMoves(piece, new[] { (-1, 0), (1, 0), (0, -1), (0, 1) }));
                break;
            case ChessPiece.PieceType.Knight:
                moves.AddRange(GetKnightMoves(piece));
                break;
            case ChessPiece.PieceType.Bishop:
                moves.AddRange(GetLinearMoves(piece, new[] { (-1, -1), (-1, 1), (1, -1), (1, 1) }));
                break;
            case ChessPiece.PieceType.Queen:
                moves.AddRange(GetLinearMoves(piece,
                    new[] { (-1, 0), (1, 0), (0, -1), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1) }));
                break;
            case ChessPiece.PieceType.King:
                moves.AddRange(GetKingMovesWithoutCheck(piece));
                moves.AddRange(GetCastlingMoves(piece));
                break;
        }

        return moves;
    }

    /// <summary>
    ///     Returns valid moves for a pawn, including standard movement and capturing.
    /// </summary>
    private List<(int, int)> GetPawnMoves(ChessPiece piece)
    {
        var moves = new List<(int, int)>();
        var direction = piece.Color == ChessPiece.PieceColor.White ? 1 : -1;
        var startRow = piece.Color == ChessPiece.PieceColor.White ? 1 : 6;

        if (IsValidSquare(piece.Row + direction, piece.Col) && Pieces[piece.Row + direction, piece.Col] == null)
        {
            moves.Add((piece.Row + direction, piece.Col));

            if (piece.Row == startRow && Pieces[piece.Row + 2 * direction, piece.Col] == null)
                moves.Add((piece.Row + 2 * direction, piece.Col));
        }

        for (var colOffset = -1; colOffset <= 1; colOffset += 2)
            if (IsValidSquare(piece.Row + direction, piece.Col + colOffset))
            {
                var targetPiece = Pieces[piece.Row + direction, piece.Col + colOffset];
                if (targetPiece != null && targetPiece.Color != piece.Color)
                    moves.Add((piece.Row + direction, piece.Col + colOffset));
            }

        return moves;
    }

    /// <summary>
    ///     Returns valid moves for a knight, which moves in an "L" shape.
    /// </summary>
    private List<(int, int)> GetKnightMoves(ChessPiece piece)
    {
        var moves = new List<(int, int)>();
        int[] offsets = { -2, -1, 1, 2 };

        foreach (var rowOffset in offsets)
        foreach (var colOffset in offsets)
            if (Math.Abs(rowOffset) != Math.Abs(colOffset))
            {
                var newRow = piece.Row + rowOffset;
                var newCol = piece.Col + colOffset;

                if (IsValidSquare(newRow, newCol) && Pieces[newRow, newCol]?.Color != piece.Color)
                    moves.Add((newRow, newCol));
            }

        return moves;
    }

    /// <summary>
    ///     Returns valid moves for the king, excluding moves that would leave the king in check.
    /// </summary>
    private List<(int, int)> GetKingMovesWithoutCheck(ChessPiece piece)
    {
        var moves = new List<(int, int)>();

        for (var rowOffset = -1; rowOffset <= 1; rowOffset++)
        for (var colOffset = -1; colOffset <= 1; colOffset++)
            if (rowOffset != 0 || colOffset != 0)
            {
                var newRow = piece.Row + rowOffset;
                var newCol = piece.Col + colOffset;

                if (IsValidSquare(newRow, newCol) && Pieces[newRow, newCol]?.Color != piece.Color &&
                    !WouldLeaveKingInCheck(piece, (newRow, newCol)))
                    moves.Add((newRow, newCol));
            }

        return moves;
    }

    /// <summary>
    ///     Returns valid castling moves for the king.
    /// </summary>
    private List<(int, int)> GetCastlingMoves(ChessPiece king)
    {
        var castlingMoves = new List<(int, int)>();
        if (Castling.CanCastle(king.Color, true)) castlingMoves.Add((king.Row, king.Col + 2));
        if (Castling.CanCastle(king.Color, false)) castlingMoves.Add((king.Row, king.Col - 2));
        return castlingMoves;
    }

    /// <summary>
    ///     Returns valid moves for linear-moving pieces like rooks, bishops, and queens.
    /// </summary>
    private List<(int, int)> GetLinearMoves(ChessPiece piece, (int, int)[] directions)
    {
        var moves = new List<(int, int)>();

        foreach (var (rowDirection, colDirection) in directions)
        {
            var newRow = piece.Row + rowDirection;
            var newCol = piece.Col + colDirection;

            while (IsValidSquare(newRow, newCol))
            {
                if (Pieces[newRow, newCol] == null)
                {
                    moves.Add((newRow, newCol));
                }
                else
                {
                    if (Pieces[newRow, newCol].Color != piece.Color) moves.Add((newRow, newCol));
                    break;
                }

                newRow += rowDirection;
                newCol += colDirection;
            }
        }

        return moves;
    }

    /// <summary>
    ///     Checks whether a given square is under attack by any opposing piece.
    /// </summary>
    private bool IsSquareUnderAttack(int row, int col, ChessPiece.PieceColor defendingColor)
    {
        var attackingColor = defendingColor == ChessPiece.PieceColor.White
            ? ChessPiece.PieceColor.Black
            : ChessPiece.PieceColor.White;

        // Check for attacks by pawns
        if (attackingColor == ChessPiece.PieceColor.White)
        {
            if (IsValidSquare(row - 1, col - 1) && Pieces[row - 1, col - 1]?.Type == ChessPiece.PieceType.Pawn &&
                Pieces[row - 1, col - 1].Color == attackingColor) return true;

            if (IsValidSquare(row - 1, col + 1) && Pieces[row - 1, col + 1]?.Type == ChessPiece.PieceType.Pawn &&
                Pieces[row - 1, col + 1].Color == attackingColor) return true;
        }
        else
        {
            if (IsValidSquare(row + 1, col - 1) && Pieces[row + 1, col - 1]?.Type == ChessPiece.PieceType.Pawn &&
                Pieces[row + 1, col - 1].Color == attackingColor) return true;

            if (IsValidSquare(row + 1, col + 1) && Pieces[row + 1, col + 1]?.Type == ChessPiece.PieceType.Pawn &&
                Pieces[row + 1, col + 1].Color == attackingColor) return true;
        }

        // Check for attacks by knights
        int[] knightMoves = { -2, -1, 1, 2 };
        foreach (var rowOffset in knightMoves)
            foreach (var colOffset in knightMoves)
                if (Math.Abs(rowOffset) != Math.Abs(colOffset))
                {
                    var newRow = row + rowOffset;
                    var newCol = col + colOffset;

                    if (IsValidSquare(newRow, newCol) && Pieces[newRow, newCol]?.Type == ChessPiece.PieceType.Knight &&
                        Pieces[newRow, newCol].Color == attackingColor)
                        return true;
                }

        // Check for attacks by bishops, rooks, and queens
        (int, int)[] directions = { (-1, -1), (-1, 1), (1, -1), (1, 1), (-1, 0), (1, 0), (0, -1), (0, 1) };
        foreach (var (dRow, dCol) in directions)
        {
            var newRow = row + dRow;
            var newCol = col + dCol;

            while (IsValidSquare(newRow, newCol))
            {
                var piece = Pieces[newRow, newCol];
                if (piece != null)
                {
                    if (piece.Color == attackingColor)
                        if ((piece.Type == ChessPiece.PieceType.Bishop && Math.Abs(dRow) == Math.Abs(dCol)) ||
                            (piece.Type == ChessPiece.PieceType.Rook && (dRow == 0 || dCol == 0)) ||
                            piece.Type == ChessPiece.PieceType.Queen)
                            return true;
                    break;
                }

                newRow += dRow;
                newCol += dCol;
            }
        }

        // Check for attacks by kings
        for (var dRow = -1; dRow <= 1; dRow++)
        for (var dCol = -1; dCol <= 1; dCol++)
            if (dRow != 0 || dCol != 0)
            {
                var newRow = row + dRow;
                var newCol = col + dCol;

                if (IsValidSquare(newRow, newCol) && Pieces[newRow, newCol]?.Type == ChessPiece.PieceType.King &&
                    Pieces[newRow, newCol].Color == attackingColor)
                    return true;
            }

        return false;
    }

    #endregion

    #region Utility Methods

    /// <summary>
    ///     Determines if a given square is valid on the board.
    /// </summary>
    private bool IsValidSquare(int row, int col)
    {
        return row >= 0 && row < 8 && col >= 0 && col < 8;
    }

    /// <summary>
    ///     Simulates a move to check if it would leave the king in check.
    /// </summary>
    private bool WouldLeaveKingInCheck(ChessPiece piece, (int toRow, int toCol) move)
    {
        var originalRow = piece.Row;
        var originalCol = piece.Col;
        var capturedPiece = Pieces[move.toRow, move.toCol];

        Pieces[move.toRow, move.toCol] = piece;
        Pieces[originalRow, originalCol] = null;
        piece.Row = move.toRow;
        piece.Col = move.toCol;

        var king = GetKing(piece.Color);
        var kingInCheck = IsSquareUnderAttack(king.Row, king.Col, king.Color);

        piece.Row = originalRow;
        piece.Col = originalCol;
        Pieces[originalRow, originalCol] = piece;
        Pieces[move.toRow, move.toCol] = capturedPiece;

        return kingInCheck;
    }

    /// <summary>
    ///     Returns the king piece of the given color.
    /// </summary>
    private ChessPiece GetKing(ChessPiece.PieceColor color)
    {
        for (var row = 0; row < 8; row++)
        for (var col = 0; col < 8; col++)
        {
            var piece = Pieces[row, col];
            if (piece != null && piece.Type == ChessPiece.PieceType.King && piece.Color == color) return piece;
        }

        throw new InvalidOperationException("King not found on the board");
    }

    /// <summary>
    ///     Returns all pieces of the given color.
    /// </summary>
    public IEnumerable<ChessPiece> GetPieces(ChessPiece.PieceColor color)
    {
        for (var row = 0; row < 8; row++)
        for (var col = 0; col < 8; col++)
            if (Pieces[row, col]?.Color == color)
                yield return Pieces[row, col];
    }

    /// <summary>
    ///     Generates the FEN representation of the current board state.
    /// </summary>
    public string GetFEN()
    {
        return
            $"{GetPiecePlacement()} {GetActiveColor()} {GetCastlingRights()} {EnPassantTarget} {HalfmoveClock} {FullmoveNumber}";
    }

    /// <summary>
    ///     Generates the FEN piece placement string for the board.
    /// </summary>
    private string GetPiecePlacement()
    {
        var fen = new StringBuilder();
        for (var row = 7; row >= 0; row--)
        {
            var emptySquares = 0;
            for (var col = 0; col < 8; col++)
            {
                var piece = Pieces[row, col];
                if (piece == null)
                {
                    emptySquares++;
                }
                else
                {
                    if (emptySquares > 0) fen.Append(emptySquares);
                    fen.Append(GetFenChar(piece));
                    emptySquares = 0;
                }
            }

            if (emptySquares > 0) fen.Append(emptySquares);
            if (row > 0) fen.Append('/');
        }

        return fen.ToString();
    }

    /// <summary>
    ///     Returns the current active player's color in FEN format ("w" or "b").
    /// </summary>
    private string GetActiveColor()
    {
        return State.ActiveColor == ChessPiece.PieceColor.White ? "w" : "b";
    }

    /// <summary>
    ///     Generates the FEN castling rights string.
    /// </summary>
    private string GetCastlingRights()
    {
        var castling = new StringBuilder();
        if (WhiteKingsideCastle) castling.Append('K');
        if (WhiteQueensideCastle) castling.Append('Q');
        if (BlackKingsideCastle) castling.Append('k');
        if (BlackQueensideCastle) castling.Append('q');
        return castling.Length > 0 ? castling.ToString() : "-";
    }

    /// <summary>
    ///     Returns the FEN character for the given piece.
    /// </summary>
    private char GetFenChar(ChessPiece piece)
    {
        var c = piece.Type switch
        {
            ChessPiece.PieceType.Pawn => 'p',
            ChessPiece.PieceType.Rook => 'r',
            ChessPiece.PieceType.Knight => 'n',
            ChessPiece.PieceType.Bishop => 'b',
            ChessPiece.PieceType.Queen => 'q',
            ChessPiece.PieceType.King => 'k',
            _ => throw new ArgumentException("Invalid piece type")
        };
        return piece.Color == ChessPiece.PieceColor.White ? char.ToUpper(c) : c;
    }

    #endregion
}