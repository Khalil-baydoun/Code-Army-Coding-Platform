import sys
t= int(input())
while t>0:
	a = []
	p = input()
	p = p.split()
	for i in p:
			a.append(int(i))
	print(a[0] + a[1]+10)
	t=t-1;
