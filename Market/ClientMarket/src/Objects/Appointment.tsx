export default class Appointment {
  id: number;
  member: string;
  role: string;
  appointer: string | null;
  appointees: string[];
  permission: string;
  constructor(
    id: number,
    member: string,
    role: string,
    appointer: string | null,
    appointees: string[],
    permission: string
  ) {
    this.id = id;
    this.member = member;
    this.appointer = appointer;
    this.appointees = appointees;
    this.role = role;
    this.permission = permission;
  }
}
