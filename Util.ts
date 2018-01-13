
function resetTime(date: any) {
    return new Date(date.setUTCHours(24, 0, 0, 0));
}

function easterDay(year: number) {
    const a = year % 19;
    const b = Math.floor(year / 100);
    const c = year % 100;
    const d = (19 * a + b - Math.floor(b / 4) - Math.floor((b - Math.floor((b + 8) / 25) + 1) / 3) + 15) % 30;
    const e = (32 + 2 * (b % 4) + 2 * Math.floor(c / 4) - d - (c % 4)) % 7;
    const f = d + e - 7 * Math.floor((a + 11 * d + 22 * e) / 451) + 114;
    const month = Math.floor(f / 31) - 1;
    const day = f % 31 + 1;
    return new Date(year, month, day);
}

function addDays(date: any, days: number) {
  date = new Date(date);
  return new Date(date.setDate(date.getDate() + days));
}

function getHolidays(year: number) {
    const holidays = [];
    const easter = resetTime(easterDay(year));
    holidays.push(resetTime(new Date(year, 0, 1))); // Første nyttårsdag
    holidays.push(resetTime(new Date(year, 4, 1))); // Første mai
    holidays.push(resetTime(new Date(year, 4, 17))); // Grunnlovsdagen
    holidays.push(resetTime(new Date(year, 11, 25))); // Første juledag
    holidays.push(resetTime(new Date(year, 11, 26))); // Andre juledag
    
    holidays.push(addDays(easter, -3)); // Skjærtorsdag
    holidays.push(addDays(easter, -2)); // Langfredag
    holidays.push(easter); // Første påskedag
    holidays.push(addDays(easter, 1)); // Andre påskedag
    holidays.push(addDays(easter, 39)); // Kristi himmelfartsdag
    holidays.push(addDays(easter, 49)); // Første pinsedag
    holidays.push(addDays(easter, 50)); // Andre pinsedag

    return holidays;
}

function isHoliday(date: any) {
		if (Object.prototype.toString.call(date) !== '[object Date]') {
    		throw new TypeError('date has not a valid type.');
    }
    
    const dateTime = resetTime(new Date(date.getFullYear(), date.getMonth(), date.getDate())).getTime();
    return getHolidays(date.getFullYear()).map(d => d.getTime()).includes(dateTime);
}
