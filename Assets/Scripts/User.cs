using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class User {
	public string deviceModel;
	public int flag;
	public string name;
	public int score;
	public int coin;

	public User(string deviceModel, int flag, string name, int score, int coin) {
		this.deviceModel = deviceModel;
		this.flag = flag;
		this.name = name;
		this.score = score;
		this.coin = coin;
	}
}