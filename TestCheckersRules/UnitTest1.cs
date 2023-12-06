using CheckersBase;
using CheckersRules;
using Matrix;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestCheckersRules
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestStartBoard()
        {
            var board = Board.CreateStartingBoard();

            var validator = Rules.FindValidMotions(board, true);

            var mtn = new Motion() { Moves = new List<Point>() { new Point(2, 5), new Point(3, 4) } };

            var validResult = validator.ValidateMotion(mtn);

            Assert.IsTrue(validResult == MotionValidationEnum.VALID);
        }

        [TestMethod]
        public void TestKingKills()
        {
            var board = Board.CreateStartingBoard();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board[i, j] = new BlankPiece();

            board[4, 0] = new WhiteKing();
            board[3, 1] = new BlackPawn();

            var validator = Rules.FindValidMotions(board, true);

            var mtn = new Motion(new Point(4, 0), new Point(0, 4));

            Assert.IsTrue(validator.ValidateMotion(mtn) == MotionValidationEnum.VALID);

            var newBoard = Rules.ApplyMotion(board, mtn, true);

            Assert.IsTrue(newBoard[3, 1].Type == PieceTypes.None);
        }

        [TestMethod]
        public void TestBug()
        {
            var board = Board.CreateStartingBoard();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board[i, j] = new BlankPiece();

            board[1, 0] = new BlackPawn();
            board[3, 0] = new BlackPawn();
            board[5, 0] = new BlackPawn();
            board[7, 0] = new BlackPawn();
            board[2, 1] = new BlackPawn();
            board[6, 1] = new BlackPawn();
            board[7, 2] = new BlackPawn();
            board[4, 3] = new BlackPawn();

            board[1, 4] = new WhitePawn();
            board[4, 5] = new WhitePawn();
            board[1, 6] = new WhitePawn();
            board[0, 7] = new WhitePawn();
            board[2, 7] = new WhitePawn();
            board[6, 7] = new BlackKing();

            bool isWhite = false;

            var smith = new AgentSmith();

            var mtn = smith.FindMotion(board, isWhite);

            var validator = Rules.FindValidMotions(board, isWhite);

            Assert.IsTrue(validator.ValidateMotion(mtn) == MotionValidationEnum.VALID);

        }

        [TestMethod]
        public void TestErr2()
        {
            var board = Board.CreateStartingBoard();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board[i, j] = new BlankPiece();

            board[3, 0] = new WhiteKing();
            board[2, 1] = new BlackPawn();
            board[1, 2] = new BlackPawn();
            board[6, 3] = new BlackPawn();

            bool isWhite = true;
            var smith = new AgentSmith();
            var mtn = smith.FindMotion(board, isWhite);

            var validator = Rules.FindValidMotions(board, isWhite);
            
            Assert.IsTrue(mtn.Moves.SequenceEqual(new List<Point>(){new Point(3,0), new Point(7,4)}));

        }

        [TestMethod]
        public void TestErr3()
        {
            var board = Board.CreateStartingBoard();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board[i, j] = new BlankPiece();

            board[7, 0] = new BlackPawn();
            
            board[3, 2] = new BlackPawn();
            board[5, 2] = new BlackPawn();
            
            board[2, 3] = new BlackPawn();
            board[4, 3] = new BlackPawn();
            
            board[1, 4] = new BlackPawn();
            board[3, 4] = new BlackPawn();
            
            board[6, 5] = new WhitePawn();
            board[7, 6] = new WhitePawn();

            board[0, 7] = new BlackKing();
            board[2, 7] = new WhiteKing();

            bool isWhite = true;
            var smith = new AgentSmith();
            var mtn = smith.FindMotion(board, isWhite);

            var validator = Rules.FindValidMotions(board, isWhite);

            Assert.IsFalse(mtn.Moves.SequenceEqual(new List<Point>() { new Point(0, 7), new Point(1, 6) }));
        }

        [TestMethod]
        public void TestErr4()
        {
            var board = Board.CreateStartingBoard();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board[i, j] = new BlankPiece();

           
            board[2, 1] = new BlackPawn();
            board[1, 2] = new BlackPawn();
            board[0, 7] = new BlackPawn();

            board[2, 7] = new WhiteKing();
            board[1, 6] = new BlackKing();

            bool isWhite = true;
            var smith = new AgentSmith();
            var mtn = smith.FindMotion(board, isWhite);

            var validator = Rules.FindValidMotions(board, true);

            Assert.IsTrue(mtn.Moves.SequenceEqual(new List<Point>() { new Point(2, 7), new Point(0, 5) }));

        }

        [TestMethod]
        public void TestErr4_1()
        {
            var board = Board.CreateStartingBoard();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board[i, j] = new BlankPiece();


            board[2, 1] = new BlackPawn();
            board[1, 2] = new BlackPawn();
            board[0, 7] = new BlackPawn();

            board[2, 7] = new WhiteKing();
            board[1, 6] = new BlackPawn();
            board[0, 5] = new BlackPawn();


            var validator = Rules.FindValidMotions(board, true);

            Assert.IsTrue(validator.GetAllMotions().ToList().Count == 5);

        }

        [TestMethod]
        public void TestErr7()
        {
            var board = Board.CreateStartingBoard();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board[i, j] = new BlankPiece();


            board[4, 5] = new BlackPawn();
            board[6, 3] = new BlackPawn();
            board[6, 5] = new BlackPawn();
            
            board[7, 6] = new WhiteKing();
            
            var validator = Rules.FindValidMotions(board, true);

            Assert.IsTrue(validator.GetAllMotions().Count == 3);

        }


        [TestMethod]
        public void TestErr8()
        {
            var board = Board.CreateStartingBoard();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board[i, j] = new BlankPiece();


            board[0, 1] = new BlackPawn();
            board[1, 2] = new BlackPawn();
            board[0, 3] = new BlackPawn();

            board[6, 1] = new BlackPawn();
            board[5, 2] = new BlackPawn();
            board[7, 2] = new BlackPawn();
            board[6, 3] = new BlackPawn();
            board[5, 4] = new BlackPawn();
            board[6, 5] = new BlackPawn();

            board[7, 6] = new WhitePawn();
            board[0, 5] = new WhitePawn();
            board[2, 5] = new WhitePawn();

            board[2, 7] = new BlackKing();

            bool isWhite = false;
            var smith = new AgentSmith();
            var mtn = smith.FindMotion(board, isWhite);

            var validator = Rules.FindValidMotions(board, isWhite);

            Assert.IsTrue(validator.GetAllMotions().ToList().Count == 9);

        }

        [TestMethod]
        public void TestErr8_1()
        {
            var board = Board.CreateStartingBoard();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board[i, j] = new BlankPiece();


            board[0, 1] = new BlackPawn();
            board[1, 2] = new BlackPawn();
            board[1, 4] = new BlackPawn();

            board[0, 5] = new WhitePawn();
            board[2, 5] = new WhitePawn();

            bool isWhite = true;

            var validator = Rules.FindValidMotions(board, isWhite);

            Assert.IsTrue(validator.GetAllMotions().ToList().Count == 2);

        }

        [TestMethod]
        public void TestErr10()
        {
            var board = Board.CreateStartingBoard();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board[i, j] = new BlankPiece();


            board[7, 0] = new BlackPawn();
            board[0, 1] = new BlackPawn();
            board[4, 1] = new BlackPawn();
            board[6, 1] = new BlackPawn();
            board[1, 2] = new BlackPawn();
            board[7, 2] = new BlackPawn();
            board[0, 3] = new BlackPawn();
            board[2, 3] = new BlackPawn();
            board[6, 3] = new BlackPawn();
            board[7, 4] = new BlackPawn();

            board[3, 2] = new WhitePawn();
            board[3, 4] = new WhitePawn();
            board[5, 4] = new WhitePawn();
            board[0, 5] = new WhitePawn();
            board[2, 5] = new WhitePawn();
            board[4, 5] = new WhitePawn();
            board[5, 6] = new WhitePawn();
            board[0, 7] = new WhitePawn();
            board[2, 7] = new WhitePawn();
            board[4, 7] = new WhitePawn();

            bool isWhite = false;
            var smith = new AgentSmith();
            var mtn = smith.FindMotion(board, isWhite);

            var validator = Rules.FindValidMotions(board, isWhite);

            Assert.IsTrue(false);

        }
    }
}
