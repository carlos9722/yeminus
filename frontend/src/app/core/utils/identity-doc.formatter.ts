export function stripIdentityDocDigits(value: string): string {
  return value.replace(/\D/g, '');
}

export function formatIdentityDoc(value: string): string {
  const digits = value.replace(/\D/g, '');
  if (digits.length === 0) {
    return '';
  }

  const first = digits.slice(0, 1);
  const rest = digits.slice(1);
  const groups: string[] = [first];

  for (let i = 0; i < rest.length; i += 3) {
    groups.push(rest.slice(i, i + 3));
  }

  return groups.filter((g) => g.length > 0).join('.');
}
