export class Stats {
  scripts: Scripts;
  rules: Rules;
  catalog: Catalog;
}

export class Scripts {
  scan: string;
  catalogMain: string;
  catalogSpam: string;
  markForDelete: string;
  removeMain: string;
  removeSpam: string;
}

export class Rules {
  domain: RulesCounter;
  sender: RulesCounter;
  subject: RulesCounter;
}

export class RulesCounter {
  count: number;
  used: number;
}

export class Catalog {
  totalEntries: number;
  readyToRemove: number;
  removed: number;
  new: number;
}
