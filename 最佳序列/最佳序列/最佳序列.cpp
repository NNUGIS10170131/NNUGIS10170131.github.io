// 最佳序列.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include "pch.h"
#include <iostream>
#include<algorithm>
#include<stdio.h>
using namespace std;

//采用结构类来表示工作
struct Job {
	char id;      // 工作编号
	int cost;    //花费时间
	int dead;    // 截止时间
	int profit;  //完成工作获得的利益
};

//该函数用于按利益大小排序
bool comparison(Job a, Job b)
{
	return (a.profit > b.profit);
}

// 计算最佳工作序列
void printJobScheduling(Job arr[], int n)
{
	// 按利益递减次序对工作队列排序
	sort(arr, arr + n, comparison);

	//统计最大期限
	int T = 0;
	for (int i = 0; i < n; i++)
	{
		T = max(T, arr[i].dead);
	}

	int *result = new int[T]; // 保存结果序列，记录对应的工作数组下标
	int *slot = new int[T];  // 记录对应时间窗口是否被占用

	int t = 0;//工作序列花费时间
	int worth = 0;//工作序列总价值

	 // 初始化时间数组为-1						
	for (int i = 0; i < T; i++)
		slot[i] = -1;

	// 对排好序的队列依次执行
	for (int i = 0; i < n; i++) {
		// 试图为当前工作找到可用的最晚的空闲时间窗口
		// 从晚到早依次检查	
		for (int j = arr[i].dead - 1; j >= 0; j--) {
			// 找一个空闲窗口,判断是否有足够的时间空隙
			if (slot[j] == -1) {
				//进行标记
				int flag = 0;
				//如果空位少于花费时间，标记为1，并退出
				for (int k = j; k >= j - arr[i].cost + 1; k--) {
					if (j - arr[i].cost + 1 < 0) {
						flag = 1;
						break;
					}
					if (slot[k] != -1)
						flag = 1;
				}
				//当时间足够，将时间数组命名为工作引导号
				if (flag == 0) {
					for (int k = j; k >= j - arr[i].cost + 1; k--)
						slot[k] = i;
					break;
				}
			}
		}
	}
	//录入结果
	for (int i = 0; i < n; i++) {
		result[i] = -1;
	}
	for (int i = 0, j = 0; i < n && j < T; j++) {
		if (j == 0) {
			if (slot[j] != -1)
			{
				result[i] = arr[slot[j]].id;
				t += arr[slot[j]].cost;
				worth += arr[slot[j]].profit;
				i++;
				continue;
			}
		}
		if (slot[j] == -1)
			continue;
		else if (slot[j] != slot[j - 1]) {
			result[i] = slot[j];
			t += arr[slot[j]].cost;
			worth += arr[slot[j]].profit;
			i++;
			continue;
		}
	}
	// 输出结果
	for (int i = 0; i < n; i++)
	{
		if (result[i] != -1)
			cout << arr[result[i]].id << " ";
	}
	cout << endl;
	cout << "total cost:" << t << endl;
	cout << "total worth:" << worth << endl;;
}

// Driver program to test methods
int main() {
	int n = 4;
	Job arr[] = { { '1',4, 10, 2 },{ '2',3, 8, 3 },{ '3',2,4, 3 },
	{ '4', 1,2, 6 } };

	cout << " " << endl;
	cout << "ID" << "\t" << "Cost" << "\t" << "DDL" << "\t" << "Profit" << endl;
	for (int i = 0; i < n; i++) {
		cout << arr[i].id << "\t" << arr[i].cost << "\t" << arr[i].dead << "\t" << arr[i].profit << endl;;
	}

	cout << "Following is maximum profit sequence of jobs\n";
	printJobScheduling(arr, n);
	getchar();
	return 0;
}


// 运行程序: Ctrl + F5 或调试 >“开始执行(不调试)”菜单
// 调试程序: F5 或调试 >“开始调试”菜单

// 入门提示: 
//   1. 使用解决方案资源管理器窗口添加/管理文件
//   2. 使用团队资源管理器窗口连接到源代码管理
//   3. 使用输出窗口查看生成输出和其他消息
//   4. 使用错误列表窗口查看错误
//   5. 转到“项目”>“添加新项”以创建新的代码文件，或转到“项目”>“添加现有项”以将现有代码文件添加到项目
//   6. 将来，若要再次打开此项目，请转到“文件”>“打开”>“项目”并选择 .sln 文件
