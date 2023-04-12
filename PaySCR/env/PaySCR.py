import sys
from SimpleQIWI import *


token = sys.argv[1]
phone = sys.argv[2]
prise = sys.argv[4]
numberTarget = sys.argv[3]
print("- Transfer of funds to the number" + numberTarget)
api = QApi(token = token,phone = phone)
api.pay(account = numberTarget,amount = prise,comment = "Withdrawal of funds from Russian Mining")
