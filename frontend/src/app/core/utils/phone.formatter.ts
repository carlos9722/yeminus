export function extractPhoneDigits(storedPhone: string): string {
  return storedPhone.replace(/\D/g, '').replace(/^57/, '').slice(0, 10);
}
