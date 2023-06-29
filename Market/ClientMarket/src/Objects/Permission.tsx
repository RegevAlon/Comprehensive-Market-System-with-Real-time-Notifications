enum Permission {
  ManageSupply = 1,
  Appoint = 2,
  Policy = 4,
  UserApplications = 8,
  ShopPurchaseHistory = 16,
  ShopAppointmentsInfo = 32,
  OpenCloseShop = 64,
  BidsPermissions = 128,
  All = ManageSupply |
    Appoint |
    Policy |
    UserApplications |
    ShopPurchaseHistory |
    ShopAppointmentsInfo |
    OpenCloseShop |
    BidsPermissions,
}
export default Permission;

export function permissionToString(permissionNum: number): string {
  switch (permissionNum) {
    case 0:
      return "Manage Supply";
    case 1:
      return "Appoint";
    case 2:
      return "Policies";
    case 3:
      return "User Applications";
    case 4:
      return "Recieve Shop History";
    case 5:
      return "Recieve Appointments Info";
    case 6:
      return "Open/Close Shop";
    case 7:
      return "Bids Permissions";
    case 8:
      return "All Permissions";
  }
}
