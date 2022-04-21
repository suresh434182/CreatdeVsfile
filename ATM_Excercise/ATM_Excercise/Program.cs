using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM_Excercise
{
	class Program
	{
		static void Main(string[] args)
		{
			ATM theATM = new ATM();
			theATM.run();
		}
	}
	public class ATM
	{

		private bool userAuthenticated;
		private int currentAccountNumber;
		private Screen screen;
		private Keypad keypad;
		private CashDispenser cashDispenser;
		private DepositSlot depositSlot;
		private BankDatabase bankDatabase;
		private enum MenuOption
		{
			BALANCE_INQUIRY = 1,
			WITHDRAWAL = 2,
			DEPOSIT = 3,
			EXIT_ATM = 4
		}
		public ATM()
		{
			userAuthenticated = false;
			currentAccountNumber = 0;
			screen = new Screen();
			keypad = new Keypad();
			cashDispenser = new CashDispenser();
			depositSlot = new DepositSlot();
			bankDatabase = new BankDatabase();
		}
		public void run()
		{
			while (true)
			{
				while (!userAuthenticated)
				{
					screen.DisplayMessageLine("\nWelcome!");
					AuthenticateUser();
				}
				PerformTransactions();
				userAuthenticated = false;
				currentAccountNumber = 0;
				screen.DisplayMessageLine("\nThank you! Goodbye!");
			}
		}
		private void AuthenticateUser()
		{

			screen.DisplayMessage("\nPlease enter your account number: ");
			int accountNumber = keypad.GetInput();


			screen.DisplayMessage("\nEnter your PIN: ");
			int pin = keypad.GetInput();


			userAuthenticated = bankDatabase.AuthenticateUser(accountNumber, pin);
			if (userAuthenticated)
				currentAccountNumber = accountNumber;
			else
				screen.DisplayMessageLine("Invalid account number or PIN. Please try again.");
		}


		private void PerformTransactions()
		{
			Transaction currentTransaction;
			bool userExited = false;


			while (!userExited)
			{

				int mainMenuSelection = DisplayMainMenu();


				switch ((MenuOption)mainMenuSelection)
				{

					case MenuOption.BALANCE_INQUIRY:
					case MenuOption.WITHDRAWAL:
					case MenuOption.DEPOSIT:

						currentTransaction =
						CreateTransaction(mainMenuSelection);
						currentTransaction.Execute();
						break;
					case MenuOption.EXIT_ATM:
						screen.DisplayMessageLine("\nExiting the system...");
						userExited = true;
						break;
					default:
						screen.DisplayMessageLine("\nYou did not enter a valid selection. Try again.");
						break;
				}
			}
		}


		private int DisplayMainMenu()
		{
			screen.DisplayMessageLine("\nMain menu:");
			screen.DisplayMessageLine("1 - View my balance");
			screen.DisplayMessageLine("2 - Withdraw cash");
			screen.DisplayMessageLine("3 - Deposit funds");
			screen.DisplayMessageLine("4 - Exit\n");
			screen.DisplayMessage("Enter a choice: ");
			return keypad.GetInput();
		}


		private Transaction CreateTransaction(int type)
		{
			Transaction temp = null;


			switch ((MenuOption)type)
			{

				case MenuOption.BALANCE_INQUIRY:
					temp = new BalanceInquiry(currentAccountNumber,
					screen, bankDatabase);
					break;
				case MenuOption.WITHDRAWAL:
					temp = new Withdrawal(currentAccountNumber, screen, bankDatabase, keypad, cashDispenser);
					break;
				case MenuOption.DEPOSIT:
					temp = new Deposit(currentAccountNumber, screen, bankDatabase, keypad, depositSlot);
					break;
			}

			return temp;
		}
	}
	internal class Screen
	{
		public void DisplayMessage(string message)
		{
			Console.WriteLine(message);
		}
		public void DisplayMessageLine(string message)
		{
			Console.WriteLine(message);
		}
		public void DispalyAmount(decimal amount)
		{
			Console.WriteLine("{0:c}", amount);
		}
	}
	internal class Keypad
	{
		public int GetInput()
		{
			return int.Parse(Console.ReadLine());
		}
	}
	internal class Account
	{
		int accountNumber;
		int pin;
		decimal availableBalance;
		public Account(int theAccountNumber, int thePin, decimal theAvailableBalance)
		{
			this.accountNumber = theAccountNumber;
			this.pin = thePin;
			this.availableBalance = theAvailableBalance;

		}
		public int AccountNumber { get { return accountNumber; } }
		public int Pin { get { return pin; } }
		public decimal AvailableBalance { get { return availableBalance; } }

		public bool validPin(int userPin)
		{
			return pin == userPin;
		}
		public void creadit(decimal amount)
		{
			availableBalance += amount;
		}
		public void Debit(decimal amount)
		{
			availableBalance -= amount;

		}
	}
	internal class CashDispenser
	{
		const int INITTAL_COUNT = 500;
		int billCount;
		public CashDispenser()
		{
			billCount = INITTAL_COUNT;
		}
		public void DispenseCash(decimal amount)
		{
			int billsRequired = ((int)amount) / 20;
			billCount -= billsRequired;
		}
		public bool IsSufficientCashAvailable(decimal amount)
		{
			int billsReuired = (int)amount / 20;
			return (billCount >= billsReuired);
		}
	}
	internal abstract class Transaction
	{
		int accountNumber;
		Screen userScreen;
		BankDatabase bankDatabase;
		public Transaction(int userAccount, Screen theSceen, BankDatabase theDatabase)
		{
			accountNumber = userAccount;
			bankDatabase = theDatabase;
			userScreen = theSceen;
		}
		public int AccountNumber
		{
			get { return accountNumber; }
		}
		public Screen UserScreen { get { return userScreen; } }
		public BankDatabase BankDatabase { get { return bankDatabase; } }
		public abstract void Execute();
	}
	internal class BankDatabase
	{
		private Account[] accounts;
		public BankDatabase()
		{
			accounts = new Account[2];
			accounts[0] = new Account(12345, 5432, 1000.00M);
			accounts[1] = new Account(98756, 5673, 2000.00M);
		}
		private Account GetAccount(int accountNumber)
		{
			foreach (Account currentaccount in accounts)
			{
				if (currentaccount.AccountNumber == accountNumber)
					return currentaccount;
			}
			return null;
		}
		public bool AuthenticateUser(int userAccountNumber, int UserPIN)
		{
			Account useraccount = GetAccount(userAccountNumber);
			if (useraccount != null)
			{
				return useraccount.validPin(UserPIN);


			}
			else
			{
				return false;
			}
		}
		public decimal GetAvailableBalence(int userAccountNumber)
		{
			Account userAccount = GetAccount(userAccountNumber);
			return userAccount.AvailableBalance;

		}

		public void Credit(int userAccountNumber, decimal amount)
		{
			Account userAccount = GetAccount(userAccountNumber);
			userAccount.creadit(amount);
		}
		public void Debit(int userAccountNumber, decimal amount)
		{
			Account userAccount = GetAccount(userAccountNumber);
			userAccount.Debit(amount);
		}

	}
	internal class Deposit : Transaction
	{
		decimal amount;
		Keypad keypad;
		DepositSlot depositSlot;

		const int CANCELED = 0;
		public Deposit(int userAccountNumber, Screen atmScreen,
			BankDatabase atmBankDatabase, Keypad atmKeypad,
				DepositSlot atmDepositSlot)
			: base(userAccountNumber, atmScreen, atmBankDatabase)
		{
			keypad = atmKeypad;
			depositSlot = atmDepositSlot;
		}
		public override void Execute()
		{
			amount = PromptForDepositAmount();


			if (amount != CANCELED)
			{
				DateTime ft = DateTime.Now;
				UserScreen.DisplayMessage("\nPlease insert a deposit envelope containing ");
				UserScreen.DispalyAmount(amount);
				UserScreen.DisplayMessageLine(" in the deposit slot.");


				bool envelopeReceived = depositSlot.IsDepositEnvelopReceived();


				if (envelopeReceived)
				{
					UserScreen.DisplayMessageLine($"Date And time while Done Transaction :- {ft:f}");
					UserScreen.DisplayMessageLine("\nYour envelope has been received.\n" + "The money just deposited will not be available " + "until we \nverify the amount of any " + "enclosed cash, and any enclosed checks clear.");
					BankDatabase.Credit(AccountNumber, amount);
				}
				else
					UserScreen.DisplayMessageLine("\nYou did not insert an envelope, so the ATM has " + "canceled your transaction.");
			}
			else
				UserScreen.DisplayMessageLine("\nCanceling transaction...");
		}


		private decimal PromptForDepositAmount()
		{

			UserScreen.DisplayMessage("\nPlease input a deposit amount in CENTS (or 0 to cancel): ");
			int input = keypad.GetInput();


			if (input == CANCELED)
				return CANCELED;
			else
				return input / 100.00M;
		}
	}
	internal class Withdrawal : Transaction
	{
		decimal amount;
		Keypad Keypad;
		CashDispenser cashDispenser;
		const int CANCEL = 6;
		public Withdrawal(int userAccount, Screen atmScreen, BankDatabase atmdataBase, Keypad atmKepad, CashDispenser atmcashDispenser)
			: base(userAccount, atmScreen, atmdataBase)

		{
			Keypad = atmKepad;
			cashDispenser = atmcashDispenser;
		}
		public override void Execute()
		{
			bool cashDispensed = false;
			bool transactionCanceled = false;
			do
			{
				int selection = DisplayMenuOfAmounts();
				DateTime ft = DateTime.Now;
				if (selection != CANCEL)
				{
					amount = selection;
					decimal availableBanlance = BankDatabase.GetAvailableBalence(AccountNumber);
					if (amount <= availableBanlance)
					{
						if (cashDispenser.IsSufficientCashAvailable(amount))
						{
							BankDatabase.Debit(AccountNumber, amount);

							cashDispenser.DispenseCash(amount);
							cashDispensed = true;
							UserScreen.DisplayMessageLine($"Date And time while Done Transaction :- {ft:f}");
							UserScreen.DisplayMessageLine("\nPlease take your cash from the cash dispenser.");
						}
						else
							UserScreen.DisplayMessageLine("\nInsufficient cash available in the ATM." + "\n\nPlease choose a smaller amount.");
					}
					else
						UserScreen.DisplayMessageLine("\nInsufficient cash available in your account." + "\n\nPlease choose a smaller amount.");
				}
				else
				{
					UserScreen.DisplayMessageLine("\nCanceling transaction...");
					transactionCanceled = true;
				}

			} while ((!cashDispensed) && (!transactionCanceled));
		}
		private int DisplayMenuOfAmounts()
		{
			int userChoice = 0;
			int[] amount = { 0, 20, 40, 60, 100, 200 };

			while (userChoice == 0)
			{
				UserScreen.DisplayMessageLine("\nWithdrawal options:");
				UserScreen.DisplayMessageLine("1 - $20");
				UserScreen.DisplayMessageLine("2 - $40");
				UserScreen.DisplayMessageLine("3 - $60");
				UserScreen.DisplayMessageLine("4 - $100");
				UserScreen.DisplayMessageLine("5 - $200");
				UserScreen.DisplayMessageLine("6 - Cancel transaction");
				UserScreen.DisplayMessage("\nChoose a withdrawal option (1-6): ");
				int input = Keypad.GetInput();
				switch (input)
				{
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
						userChoice = amount[input];
						break;
					case CANCEL:
						userChoice = CANCEL;
						break;
					default:

						UserScreen.DisplayMessageLine("\nInvalid selection. Try again.");
						break;
				}
			}
			return userChoice;
		}
	}
	internal class DepositSlot
	{
		public bool IsDepositEnvelopReceived()
		{
			return true;
		}
	}
	internal class BalanceInquiry : Transaction
	{
		public BalanceInquiry(int UserAccountNumber, Screen atmScreen, BankDatabase atmbankDatabase)
			: base(UserAccountNumber, atmScreen, atmbankDatabase) { }

		public override void Execute()
		{
			DateTime ft = DateTime.Now;
			decimal availableBalance = BankDatabase.GetAvailableBalence(AccountNumber);


			UserScreen.DisplayMessageLine("\nBalance Information:");
			UserScreen.DisplayMessage($" - Available balance on this time {ft:f}");

			UserScreen.DispalyAmount(availableBalance);


			UserScreen.DisplayMessageLine("");
		}
	}
}
