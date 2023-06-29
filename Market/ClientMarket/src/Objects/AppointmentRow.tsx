export default class AppointmentRow {
  member: string;
  appointer: string;
  // appointees: string[];
  constructor(
    member: string,
    appointer: string
    // appointees: string[],
  ) {
    this.member = member;
    this.appointer = appointer;
    // this.appointees = appointees;
  }
}
