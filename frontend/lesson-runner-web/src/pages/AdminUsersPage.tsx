import { useEffect, useState } from "react";
import { ApiError } from "../api/client";
import { createParentLink, deleteParentLink, getParentLinks } from "../api/parentApi";
import { getParticipants } from "../api/participantsApi";
import { Button } from "../components/ui/Button";
import { useUsersViewModel } from "../features/users/useUsersViewModel";
import type { ParentParticipantLink } from "../types/parent";
import type { ParticipantSummary } from "../types/participant";
import type { ManagedUser, UpdateUserProfileRequest } from "../types/user";

function roleLabel(role: string): string {
  if (role === "admin") return "Administrator";
  if (role === "instructor") return "Instruktor";
  if (role === "parent") return "Rodzic";
  return role;
}

type UserRowProps = {
  user: ManagedUser;
  busy: boolean;
  onToggleActive: (user: ManagedUser) => void;
  onResetPassword: (id: string, password: string) => Promise<boolean>;
  onSaveProfile: (id: string, request: UpdateUserProfileRequest) => Promise<boolean>;
  onInvite: (user: ManagedUser) => Promise<boolean>;
};

function UserRow({ user, busy, onToggleActive, onResetPassword, onSaveProfile, onInvite }: UserRowProps) {
  const [mode, setMode] = useState<"none" | "password" | "profile">("none");
  const [password, setPassword] = useState("");
  const [firstName, setFirstName] = useState(user.firstName ?? "");
  const [lastName, setLastName] = useState(user.lastName ?? "");
  const [phone, setPhone] = useState(user.phone ?? "");

  async function submitPassword() {
    const ok = await onResetPassword(user.id, password);
    if (ok) {
      setPassword("");
      setMode("none");
    }
  }

  async function submitProfile() {
    const ok = await onSaveProfile(user.id, {
      firstName: firstName.trim() || null,
      lastName: lastName.trim() || null,
      phone: phone.trim() || null,
    });
    if (ok) {
      setMode("none");
    }
  }

  return (
    <tr>
      <td>
        <strong>{user.displayName}</strong>
        {user.displayName !== user.email ? <div className="cell-sub">{user.email}</div> : null}
      </td>
      <td>{user.phone ?? "—"}</td>
      <td>{roleLabel(user.role)}</td>
      <td>
        <span className={`status-pill status-${user.isActive ? "completed" : "cancelled"}`}>
          {user.isActive ? "Aktywne" : "Wyłączone"}
        </span>
      </td>
      <td>
        {mode === "password" ? (
          <div className="session-actions">
            <input
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              placeholder="Nowe hasło (min. 8)"
              aria-label="Nowe hasło"
            />
            <Button variant="secondary" onClick={submitPassword} disabled={busy}>Zapisz hasło</Button>
            <Button variant="ghost" onClick={() => { setMode("none"); setPassword(""); }}>Anuluj</Button>
          </div>
        ) : mode === "profile" ? (
          <div className="session-actions">
            <input value={firstName} onChange={(event) => setFirstName(event.target.value)} placeholder="Imię" aria-label="Imię" />
            <input value={lastName} onChange={(event) => setLastName(event.target.value)} placeholder="Nazwisko" aria-label="Nazwisko" />
            <input value={phone} onChange={(event) => setPhone(event.target.value)} placeholder="Telefon" aria-label="Telefon" />
            <Button variant="secondary" onClick={submitProfile} disabled={busy}>Zapisz profil</Button>
            <Button variant="ghost" onClick={() => setMode("none")}>Anuluj</Button>
          </div>
        ) : (
          <div className="session-actions">
            <Button variant="ghost" onClick={() => setMode("profile")}>Edytuj profil</Button>
            <Button variant="ghost" onClick={() => onInvite(user)} disabled={busy || !user.isActive}>
              Wyślij zaproszenie
            </Button>
            <Button variant="ghost" onClick={() => setMode("password")}>Ustaw hasło</Button>
            <Button variant="ghost" onClick={() => onToggleActive(user)} disabled={busy}>
              {user.isActive ? "Wyłącz" : "Włącz"}
            </Button>
          </div>
        )}
      </td>
    </tr>
  );
}

export function AdminUsersPage() {
  const vm = useUsersViewModel();
  const [participants, setParticipants] = useState<ParticipantSummary[]>([]);
  const [links, setLinks] = useState<ParentParticipantLink[]>([]);
  const [parentUserId, setParentUserId] = useState("");
  const [participantId, setParticipantId] = useState("");
  const [relation, setRelation] = useState("");
  const [isPrimaryContact, setIsPrimaryContact] = useState(true);
  const [receivesNotifications, setReceivesNotifications] = useState(true);
  const [linkError, setLinkError] = useState<string | null>(null);

  async function loadLinks() {
    try {
      setLinkError(null);
      const [loadedParticipants, loadedLinks] = await Promise.all([getParticipants(undefined, false), getParentLinks()]);
      setParticipants(loadedParticipants);
      setLinks(loadedLinks);
    } catch (caught) {
      setLinkError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać powiązań rodziców.");
    }
  }

  useEffect(() => {
    void loadLinks();
  }, []);

  async function addLink() {
    try {
      setLinkError(null);
      await createParentLink({
        parentUserId,
        participantId,
        relation: relation.trim() || null,
        isPrimaryContact,
        receivesNotifications,
      });
      setParentUserId("");
      setParticipantId("");
      setRelation("");
      await loadLinks();
    } catch (caught) {
      setLinkError(caught instanceof ApiError ? caught.message : "Nie udało się powiązać rodzica.");
    }
  }

  async function removeLink(link: ParentParticipantLink) {
    try {
      setLinkError(null);
      await deleteParentLink(link.parentUserId, link.participantId);
      await loadLinks();
    } catch (caught) {
      setLinkError(caught instanceof ApiError ? caught.message : "Nie udało się usunąć powiązania.");
    }
  }

  const parentUsers = vm.users.filter((user) => user.role === "parent");

  return (
    <section className="page-section">
      <div className="page-header">
        <div>
          <span className="eyebrow">Administrator</span>
          <h1>Konta</h1>
          <p>Twórz konta trenerów i administratorów (imię, nazwisko, telefon) oraz włączaj/wyłączaj dostęp.</p>
        </div>
      </div>

      {vm.error ? <div className="list-state list-state-error">{vm.error}</div> : null}
      {vm.notice ? <div className="list-state">{vm.notice}</div> : null}

      <div className="editor-fieldset" style={{ marginBottom: 20 }}>
        <legend>Nowe konto</legend>
        <div className="participant-fields">
          <label className="form-field"><span>Imię</span><input value={vm.firstName} onChange={(e) => vm.setFirstName(e.target.value)} /></label>
          <label className="form-field"><span>Nazwisko</span><input value={vm.lastName} onChange={(e) => vm.setLastName(e.target.value)} /></label>
          <label className="form-field"><span>Telefon</span><input value={vm.phone} onChange={(e) => vm.setPhone(e.target.value)} /></label>
          <label className="form-field">
            <span>Rola</span>
            <select value={vm.role} onChange={(e) => vm.setRole(e.target.value)}>
              <option value="instructor">Instruktor</option>
              <option value="admin">Administrator</option>
              <option value="parent">Rodzic</option>
            </select>
          </label>
        </div>
        <div className="participant-fields">
          <label className="form-field"><span>E-mail</span><input type="email" value={vm.email} onChange={(e) => vm.setEmail(e.target.value)} /></label>
          <label className="form-field">
            <span>Hasło (zostaw puste, aby wysłać zaproszenie)</span>
            <input type="password" value={vm.password} onChange={(e) => vm.setPassword(e.target.value)} />
          </label>
        </div>
        <p className="cell-sub">
          Puste hasło = konto z zaproszeniem: użytkownik dostanie e-mail z linkiem i ustawi hasło
          sam. Nikt poza nim nie będzie go znał.
        </p>
        <div className="participant-row-actions">
          <Button onClick={vm.addUser} disabled={vm.busy}>Utwórz konto</Button>
        </div>
      </div>

      {vm.loading ? <div className="list-state">Ładowanie kont...</div> : null}

      {!vm.loading && vm.users.length > 0 ? (
        <table className="attendance-table">
          <thead>
            <tr>
              <th>Osoba</th>
              <th>Telefon</th>
              <th>Rola</th>
              <th>Status</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {vm.users.map((user) => (
              <UserRow
                key={user.id}
                user={user}
                busy={vm.busy}
                onToggleActive={vm.toggleActive}
                onResetPassword={vm.resetPassword}
                onSaveProfile={vm.saveProfile}
                onInvite={vm.invite}
              />
            ))}
          </tbody>
        </table>
      ) : null}

      <section className="editor-panel" style={{ marginTop: 20 }}>
        <div className="group-section-head">
          <div>
            <span className="eyebrow">Portal rodzica</span>
            <h2>Powiązania kont z dziećmi</h2>
          </div>
        </div>
        {linkError ? <div className="list-state list-state-error">{linkError}</div> : null}
        <div className="participant-fields">
          <label className="form-field">
            <span>Konto rodzica</span>
            <select value={parentUserId} onChange={(event) => setParentUserId(event.target.value)}>
              <option value="">Wybierz</option>
              {parentUsers.map((user) => <option key={user.id} value={user.id}>{user.displayName}</option>)}
            </select>
          </label>
          <label className="form-field">
            <span>Uczestnik</span>
            <select value={participantId} onChange={(event) => setParticipantId(event.target.value)}>
              <option value="">Wybierz</option>
              {participants.map((participant) => (
                <option key={participant.id} value={participant.id}>{participant.firstName} {participant.lastName}</option>
              ))}
            </select>
          </label>
          <div className="participant-row-actions">
            <label className="form-field">
              <span>Relacja</span>
              <input
                value={relation}
                onChange={(event) => setRelation(event.target.value)}
                placeholder="mama, tata, opiekun prawny"
                maxLength={60}
              />
            </label>
            <label className="session-notified">
              <input
                type="checkbox"
                checked={isPrimaryContact}
                onChange={(event) => setIsPrimaryContact(event.target.checked)}
              />
              <span>Pierwszy kontakt</span>
            </label>
            <label className="session-notified">
              <input
                type="checkbox"
                checked={receivesNotifications}
                onChange={(event) => setReceivesNotifications(event.target.checked)}
              />
              <span>Dostaje powiadomienia</span>
            </label>
            <Button onClick={addLink} disabled={!parentUserId || !participantId || vm.busy}>Powiąż</Button>
          </div>
        </div>
        {links.length === 0 ? <p className="cue-empty">Brak powiązań.</p> : null}
        {links.length > 0 ? (
          <table className="attendance-table">
            <thead>
              <tr>
                <th>Rodzic</th>
                <th>Uczestnik</th>
                <th>Relacja</th>
                <th>Kontakt</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {links.map((link) => (
                <tr key={`${link.parentUserId}-${link.participantId}`}>
                  <td>{vm.users.find((user) => user.id === link.parentUserId)?.displayName ?? link.parentUserId}</td>
                  <td>{participantName(participants, link.participantId)}</td>
                  <td>{link.relation || "—"}</td>
                  <td>
                    {link.isPrimaryContact ? "pierwszy kontakt" : "dodatkowy"}
                    {link.receivesNotifications === false ? " · bez powiadomień" : ""}
                  </td>
                  <td><Button variant="ghost" onClick={() => removeLink(link)}>Usuń</Button></td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : null}
      </section>
    </section>
  );
}

function participantName(participants: ParticipantSummary[], participantId: string): string {
  const participant = participants.find((item) => item.id === participantId);
  return participant ? `${participant.firstName} ${participant.lastName}` : participantId;
}
