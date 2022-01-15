using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TransformExpression
{
	static class CheckActions
	{
		public static bool IsOper(char _char) => !char.IsLetterOrDigit(_char);
		public static int GetPrior(char _char) => _char switch
		{
			'-' => 1,
			'+' => 1,
			'/' => 2,
			'*' => 2,
			'^' => 3,
			_ => 0
		};
	}
	class TransformInfix
	{
		private string Expression { get; set; }
		private int Choice { get; set; }
		public TransformInfix(string input, int choice)
		{
			Expression = input;
			Choice = choice;
		}

		void FormSelection(Stack<char> _operators, Stack<string> _operands)
		{
			char _operator = _operators.Peek();
			_operators.Pop();

			string operand_1 = _operands.Peek();
			_operands.Pop();

			string operand_2 = _operands.Peek();
			_operands.Pop();

			string result = Choice switch
			{
				1 => operand_2 + operand_1 + _operator,
				2 => _operator + operand_2 + operand_1,
				_ => null
			};
			_operands.Push(result);
		}

		void CharProcessing(char _char, Stack<char> _operators, Stack<string> _operands)
		{
			if (_char.Equals('(')) {
				_operators.Push(_char);
			}
			else if (_char.Equals(')')) {
				while (_operators.Peek() != '(' && _operators.Count != 0) {
					FormSelection(_operators, _operands);
				}
				_operators.Pop();
			}
			else if (!CheckActions.IsOper(_char)) {
				_operands.Push(_char + "");
			}
			else {
				while (_operators.Count != 0 && CheckActions.GetPrior(_operators.Peek()) >= CheckActions.GetPrior(_char)) {
					FormSelection(_operators,_operands);
				}
				_operators.Push(_char);
			}
		}
		public string Transform()
		{
			Stack<string> operands = new Stack<string>();
			Stack<char> operators = new Stack<char>();

			for (int i = 0; i < Expression.Length; i++) {
				CharProcessing(Expression[i], operators,  operands);
			}
			while (operators.Count != 0) {
				FormSelection(operators, operands);
			}
			return operands.Peek();
		}
	}
	class Program {
		public static void Main()
		{
			string pattern = @"^\w+|[+-/*^()]*$";
			Regex regex = new Regex(pattern);
			l1: Console.WriteLine("Enter an expression:");
			string input = Console.ReadLine();
			int choice;
			if (regex.IsMatch(input)) {
				do {
					Console.Write($"\n1 => Postfix form\n2 => Prefix form\n3 => New expression\n0 => Exit\nYour choice: ");
					bool success = int.TryParse(Console.ReadLine(), out choice);
					if (success) {
						switch (choice) {
							case 1:
							case 2:
								TransformInfix toPrefix = new TransformInfix(input, choice);
								Console.WriteLine($"\nResult => {toPrefix.Transform()}\n");
								break;
							case 3: Console.Write('\n'); goto l1;
							case 0: break;
						}
					}
					else throw new Exception("Invalid input!");
				} while (choice != 0);
			}
		}
	}
}