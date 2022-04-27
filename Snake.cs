using System;
using System.Threading;
using System.Runtime.InteropServices;

class Hello{
	static char move {get; set;}
	static Snake snake = new Snake();
	static Score score = new Score();
	protected class	Score{
		public int	x;
		public int	y;
		Random random = new Random();
		public void NewScore(int maxX, int maxY){
			bool stop = false;
			while (!stop){
				x = random.Next( 0, maxX);
				y = random.Next( 0, maxY);
				stop = true;
				if (snake.CanPlaceScore(x, y))
					stop = false;
			}
		}
		~Score(){
		}
	}
	class	SBody{
		public int	x;
		public int	y;
		public SBody	next;
		public SBody	prev;
		public SBody(){
			x = 0;
			y = 0;
			next = null;
			prev = null;
		}
		public SBody(SBody head, int xp, int yp){
			x = xp;
			y = yp;
			next = head;
			prev = null;
		}
		~SBody(){
		}
	}
	class	Snake{
		SBody head;
		bool gameOver;
		int	points;
		int	gameSpeed;
		public Snake(){
			head = new SBody();
			gameOver = false;
			gameSpeed = 200;
			points = 0;
		}
		public int GetSpeed(){
			return gameSpeed;
		}
		public void IncreaseGameSpeed(){
			gameSpeed -= 100;
			if (gameSpeed < 100)
				gameSpeed = 100;
		}
		public void DecreaseGameSpeed(){
			gameSpeed += 100;
			if (gameSpeed > 1000)
				gameSpeed = 1000;
		}
		public bool IsGameOver(){
			return gameOver;
		}
		public void	PointUP(){
			points++;
		}
		public int	GetPoints(){
			return points;
		}
		public bool	CanPlaceScore(int xp, int yp){
			SBody tmp = head;
			while (tmp != null){
				if (tmp.x == xp && tmp.y == yp)
					return true;
				else
					tmp = tmp.next;
			}
			return false;
		}
		public void PrintSnake(Score score){
			Console.Clear();
			SBody tmp = head;
			Console.SetCursorPosition(tmp.x, tmp.y);
			Console.Write("0");
			tmp = tmp.next;
			while(tmp != null){
				Console.SetCursorPosition(tmp.x, tmp.y);
				Console.Write("o");
				tmp = tmp.next;
			}
			Console.SetCursorPosition(score.x, score.y);
			Console.Write("#");
			Console.SetCursorPosition( 0, 0);
		}
		public void AddSnake(int xp, int yp){
			SBody newHead = new SBody(head, xp, yp);
			head.prev = newHead;
			head = newHead;
		}
		public bool SelfEat(int xp, int yp){
			SBody tmp = head;
			while(tmp != null){
				if (tmp.x == xp && tmp.y == yp){
					gameOver = true;
					move = 'e';
					return true;
				}
				else
					tmp = tmp.next;
			}
			return false;
		}
		public void Move(int xp, int yp){
			SBody tmp = head;
			while(tmp.next != null){
				tmp = tmp.next;
			}
			while(tmp.prev != null){
				tmp.x = tmp.prev.x;
				tmp.y = tmp.prev.y;
				tmp = tmp.prev;
			}
			tmp.x = xp;
			tmp.y = yp;
		}
		~Snake(){
		}
	}
	static void Control(){
		ConsoleKey key;
		while(true){
			key = Console.ReadKey().Key;
			if (key == ConsoleKey.UpArrow && move != 'd')
				move = 'u';
			else if (key == ConsoleKey.DownArrow && move != 'u')
				move = 'd';
			else if (key == ConsoleKey.LeftArrow && move != 'r')
				move = 'l';
			else if (key == ConsoleKey.RightArrow && move != 'l')
				move = 'r';
			else if (key == ConsoleKey.Subtract)
				snake.DecreaseGameSpeed();
			else if (key == ConsoleKey.Add)
				snake.IncreaseGameSpeed();
			else if (key == ConsoleKey.Escape){
				move = 'e';
				break ;
			}
			else
				move = move;
			Console.SetCursorPosition( 0, 0);
			Console.Write(" ");
		}
	}
	class Game{
		public void StartGame(int consoleW, int consoleH, ref Thread control){
			int x = 0;
			int y = 0;
			
			Console.Clear();
			score.NewScore(consoleW, consoleH);
			while(true){
				if (move == 'u')
					y--;
				else if (move == 'd')
					y++;
				else if (move == 'r')
					x++;
				else if (move == 'l')
					x--;
				else if (move == 'e' || move == 'w'){
					control.Abort();
					break ;
				}
				if (x < 0)
					x = consoleW;
				else if (x > consoleW)
					x = 0;
				if (y < 0)
					y = consoleH;
				else if (y > consoleH)
					y = 0;
				if (snake.SelfEat(x, y) || snake.GetPoints() == (consoleW + 1) * (consoleH + 1)){
					control.Abort();
					break ;
				}
				if (x == score.x && y == score.y){
					snake.AddSnake(x, y);
					snake.PointUP();
					score.NewScore(consoleW, consoleH);
				}
				else
					snake.Move(x, y);
				snake.PrintSnake(score);
				Console.SetCursorPosition( 0, 0);
				Console.Write(" ");
				Thread.Sleep(snake.GetSpeed());
			}
		}
	}
	static void Main(string[] args){
		int	consoleW;
		int	consoleH;
		Game game = new Game();
		Thread control = new Thread(new ThreadStart(Control));
	
		if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){
			Console.SetWindowSize( 32, 65);
			Console.SetBufferSize( 33, 66);
		}
		consoleW = Console.BufferWidth - 1;
		consoleH = Console.BufferHeight - 1;
		move = 'r';

		control.Start();
		game.StartGame(consoleW, consoleH, ref control);
		control.Join();
		Console.Clear();
		if (snake.IsGameOver()){
			Console.SetCursorPosition( (consoleW / 2) - 5, (consoleH / 2));
			Console.WriteLine("Game over!");
			Console.SetCursorPosition( (consoleW / 2) - 5, (consoleH / 2) + 1);
			Console.WriteLine($"Score: {snake.GetPoints().ToString()}");
			Console.SetCursorPosition( 0, 0);
			Thread.Sleep(2500);
		}
		else if (move == 'w'){
			Console.SetCursorPosition( (consoleW / 2) - 5, (consoleH / 2));
			Console.WriteLine("You win!");
			Console.SetCursorPosition( (consoleW / 2) - 5, (consoleH / 2) + 1);
			Console.WriteLine($"Score: {snake.GetPoints().ToString()}");
			Console.SetCursorPosition( 0, 0);
			Thread.Sleep(2500);
		}
		Console.Clear();
	}
}