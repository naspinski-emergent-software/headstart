In order for self registration:
- Create HS_Default_Admin admin group
- Assign all assignments where buyerID, supplierID, userID, and userGroupID values are null to HS_Default_Admin
- Assign HS_Default_Admin to all "true" admins
- Remove all security profile assignments where buyerID, supplierID, userID, and userGroupID values are null
- Create new security profile Admin_BuyerImpersonator with Role BuyerImpersonator
- Create Admin_BuyerImpersonator admin group
- Add assignment Admin_BuyerImpersonator to Admin_BuyerImpersonator group
