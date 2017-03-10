namespace ilf.pgn.Data
open Microsoft.FSharp.Collections

type Color =
    | White
    | Black

type GameResult =
    | Win of Color
    | Draw
    | Open

type PieceType =
    | King
    | Queen
    | Rook
    | Bishop
    | Knight
    | Pawn

type Piece = Color * PieceType

type File = A | B | C | D | E | F | G | H
type Rank = R1 | R2 | R3 | R4 | R5 | R6 | R7 | R8


type Square = File * Rank

type BoardSetup = {
    Board: (Piece option) [,];
    IsWhiteMove: bool;
    CanWhiteCastleKingSide: bool;
    CanWhiteCastleQueenSide: bool;
    CanBlackCastleKingSide: bool;
    CanBlackCastleQueenSide: bool;
    EnPassantSquare: Square option;
    HalfMoveClock: int;
    FullMoveCount: int;
    }
type GameInfo = string


type MoveAnnotation =
    | MindBlowing
    | Brilliant
    | Good
    | Interesting
    | Dubious
    | Mistake
    | Blunder
    | Abysmal
    | FascinatingButUnsound
    | Unclear
    | WithCompensation
    | EvenPosition
    | SlightAdvantageWhite
    | SlightAdvantageBlack
    | AdvantageWhite
    | AdvantageBlack
    | DecisiveAdvantageWhite
    | DecisiveAdvantageBlack
    | Space
    | Initiative
    | Development
    | Counterplay
    | Countering
    | Idea
    | TheoreticalNovelty
    | UnknownAnnotation

type Move = {
    OriginPiece: PieceType option;
    OriginSquare: Square;
    TargetPiece: PieceType option;
    TargetSquare: Square;
    PromotedPiece: PieceType option;
    IsCheck: bool option;
    IsDoubleCheck: bool option;
    IsCheckMate: bool option;
    Annotation: MoveAnnotation;

}

type FullMove = {
    MoveNumber: int option;
    White: Move;
    Black: Move;
}

type Entry =
    | Fullmove of FullMove
    | HalfMove of int * Move
    | GameEnd of GameResult
    | Comment of string
    | NumericAnnotationGlyph of int
    | RecursiveAnnotationVariation

type Game = {
    Event: string;
    Site: string;
    Year: int option;
    Month: int option;
    Day: int option;
    WhitePlayer: string;
    BlackPlayer: string;
    Result: GameResult;
    AdditionalInfo: GameInfo list;
    Tags: string list;
    Entries: Entry list;
    BoardSetup: BoardSetup;
    }
