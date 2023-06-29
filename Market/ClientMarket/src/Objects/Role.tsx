enum Role {
  Founder,
  Owner,
  Manager,
}
export default Role;
export function getRoleString(id: number) {
  if (id == 0) return "Founder";
  if (id == 1) return "Owner";
  if (id == 2) return "Manager";
  return "NO ROLE";
}
